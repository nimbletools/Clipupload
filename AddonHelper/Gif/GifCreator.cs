using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Gif.Components;

namespace AddonHelper {
    public class GifCreator {
        public int Width;
        public int Height;
        public Color AlphaColor = Color.Empty;

        public int Delay;
        public int ColorDept = 8;
        public int PaletteSample = 30;
        public int PaletteSize = 7;
        public int DisposeCode = -1;
        public bool HQMode = false;

        private Stream Stream;

        public void SetFPS(float fps) {
            this.Delay = (int)Math.Round(100f / fps);
        }

        private static int ToInt24(byte[] arr, int pos) {
            if (arr.Length - pos < 3) return 0;
            return BitConverter.IsLittleEndian ? (arr[pos] | arr[pos + 1] << 8 | arr[pos + 2] << 16) : (arr[pos] << 16 | arr[pos + 1] << 8 | arr[pos + 2]);
        }

        public void Start(Stream srm) {
            this.Stream = srm;

            this.WriteString("GIF89a"); // header
        }

        public void Finish() {
            this.Stream.WriteByte(0x3b); // gif trailer
            this.Stream.Flush();
        }

        public void WriteFrames(Bitmap[] imgs, Action<int> callback) {
            this.AlphaColor = Color.Empty; // DOES NOT WORK, FUCK YOU GIF.

            byte[] prevpixels = null;
            int olddelay = this.Delay;

            bool doalpha = this.AlphaColor != Color.Empty;

            this.Width = imgs[0].Width;
            this.Height = imgs[0].Height;

            for (int frame = 0; frame < imgs.Length; frame++) {
                byte[] pixels = this.GetPixels(imgs[frame]);
                byte[] optimizedpixels = new byte[pixels.Length];
                Array.Copy(pixels, optimizedpixels, pixels.Length);

                bool isdifferent = frame == 0 || frame == imgs.Length - 1; // always add first and last frame
                if (frame > 0) {
                    // should we even add this frame?
                    for (int i = 0; i < pixels.Length; i += 3) {
                        // same pixel as last time? set it to alpha

                        if (ToInt24(pixels, i) == ToInt24(prevpixels, i)) {
                            // does not work, fuck this.
                            /*if (doalpha) {
                                optimizedpixels[i + 0] = this.AlphaColor.B;
                                optimizedpixels[i + 1] = this.AlphaColor.G;
                                optimizedpixels[i + 2] = this.AlphaColor.R;
                            }*/
                        } else {
                            isdifferent = true;
                        }
                    }
                }

                if (!isdifferent) {
                    // skip this frame, add delay to make it seem like it's not skipped
                    this.Delay += olddelay;
                    if (callback != null) callback(frame);
                    continue; // don't bother doing anything else. We're skipping this shit.
                } else {
                    byte[] pixelindexes;
                    byte[] colorpalette;
                    bool[] usedpalette;
                    int alphaindex;
                    this.CreatePalette(optimizedpixels, out pixelindexes, out colorpalette, out usedpalette, out alphaindex);

                    if (frame == 0) {
                        this.WriteLSD();
                        this.WritePalette(colorpalette);
                        this.WriteRepeat();
                    }

                    this.WriteGraphicCtrlExt(alphaindex);
                    this.WriteImageDesc(frame == 0);

                    if (frame > 0)
                        this.WritePalette(colorpalette);

                    this.WritePixels(pixelindexes);

                    if (callback != null) callback(frame);
                }

                prevpixels = pixels;
                this.Delay = olddelay;
            }
        }

        private byte[] GetPixels(Bitmap img) {
            int w = img.Width;
            int h = img.Height;

            if (w != this.Width || h != this.Height) {
                // create new image with right size/format
                Bitmap temp = new Bitmap(this.Width, this.Height);
                Graphics g = Graphics.FromImage(temp);
                g.DrawImage(img, 0, 0, this.Width, this.Height);
                g.Dispose();

                img.Dispose();
                img = temp;
            }

            BitmapData bmpData = img.LockBits(new Rectangle(0, 0, this.Width, this.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            byte[] pixels = new byte[Math.Abs(bmpData.Stride) * this.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixels, 0, pixels.Length);
            img.UnlockBits(bmpData);
            img.Dispose();

            return pixels;
        }

        private void CreatePalette(byte[] pixels, out byte[] pixelindexes, out byte[] colorpalette, out bool[] usedpalette, out int alphaindex) {
            if (this.HQMode) {
                int len = pixels.Length;
                int nPix = len / 3;
                NeuQuant nq = new NeuQuant(pixels, len, this.PaletteSample);

                pixelindexes = new byte[nPix];
                usedpalette = new bool[256];
                colorpalette = nq.Process(); // thy shal cast blackmagic upon us.

                // map image pixels to new palette
                int k = 0;
                for (int i = 0; i < nPix; i++) {
                    int index = nq.Map(pixels[k++] & 0xff, pixels[k++] & 0xff, pixels[k++] & 0xff);

                    usedpalette[index] = true;
                    pixelindexes[i] = (byte)index;
                }
            } else {
                GifEncoderLQ lq = new GifEncoderLQ();
                colorpalette = lq.GetColorBytes();
                pixelindexes = new byte[pixels.Length / 3];
                usedpalette = null;
                alphaindex = 0;

                // map image pixels to new palette
                for (int i = 0; i < pixelindexes.Length; i++) {
                    long leastDistance = long.MaxValue;
                    int result = 0;

                    int r = pixels[i * 3 + 2];
                    int g = pixels[i * 3 + 1];
                    int b = pixels[i * 3 + 0];

                    for (int index = 0; index < 256; index++) {
                        int ra = r - colorpalette[index * 3 + 0];
                        int rg = g - colorpalette[index * 3 + 1];
                        int rb = b - colorpalette[index * 3 + 2];

                        long distance = ra * ra + rg * rg + rb * rb;

                        // if a difference is zero, we're good because it won't get better
                        if (distance == 0) {
                            result = index;
                            break;
                        }

                        // if a difference is the best so far, stores it as our best candidate
                        if (distance < leastDistance) {
                            leastDistance = distance;
                            result = index;
                        }
                    }

                    pixelindexes[i] = (byte)result;
                }
            }

            alphaindex = this.AlphaColor != Color.Empty ? this.FindClosest(this.AlphaColor, colorpalette) : 0;
        }

        private int FindClosest(Color c, byte[] colorpalette) {
            long leastDistance = long.MaxValue;
            int result = 0;

            for (int index = 0; index < 256; index++) {
                int ra = this.AlphaColor.R - colorpalette[index * 3 + 0];
                int rg = this.AlphaColor.G - colorpalette[index * 3 + 1];
                int rb = this.AlphaColor.B - colorpalette[index * 3 + 2];

                long distance = ra * ra + rg * rg + rb * rb;

                // if a difference is zero, we're good because it won't get better
                if (distance == 0) {
                    result = index;
                    break;
                }

                // if a difference is the best so far, stores it as our best candidate
                if (distance < leastDistance) {
                    leastDistance = distance;
                    result = index;
                }
            }

            return result;
        }

        private void WriteLSD() {
            // logical screen size
            this.WriteShort(this.Width);
            this.WriteShort(this.Height);
            // packed fields
            this.Stream.WriteByte(Convert.ToByte(0x80 | // 1   : global color table flag = 1 (gct used)
				0x70 | // 2-4 : color resolution = 7
				0x00 | // 5   : gct sort flag = 0
				this.PaletteSize)); // 6-8 : gct size

            this.Stream.WriteByte(0); // background color index
            this.Stream.WriteByte(0); // pixel aspect ratio - assume 1:1
        }

        private void WriteRepeat() {
            this.Stream.WriteByte(0x21); // extension introducer
            this.Stream.WriteByte(0xff); // app extension label
            this.Stream.WriteByte(11); // block size
            this.WriteString("NETSCAPE" + "2.0"); // app id + auth code
            this.Stream.WriteByte(3); // sub-block size
            this.Stream.WriteByte(1); // loop sub-block id
            this.WriteShort(0); // loop count (extra iterations, 0=repeat forever)
            this.Stream.WriteByte(0); // block terminator
        }

        private void WritePalette(byte[] colorpalette) {
            this.Stream.Write(colorpalette, 0, colorpalette.Length);
            int n = (3 * 256) - colorpalette.Length;

            for (int i = 0; i < n; i++) {
                this.Stream.WriteByte(0);
            }
        }

        private void WriteGraphicCtrlExt(int alphaindex) {
            this.Stream.WriteByte(0x21); // extension introducer
            this.Stream.WriteByte(0xf9); // GCE label
            this.Stream.WriteByte(4); // data block size

            this.Stream.WriteByte((byte)(this.AlphaColor == Color.Empty ? 1 : 0));
            this.WriteShort(this.Delay); // delay x 1/100 sec
            this.Stream.WriteByte((byte)alphaindex); // transparent color index

            this.Stream.WriteByte(0); // block terminator
        }

        private void WriteImageDesc(bool firstframe) {
            this.Stream.WriteByte(0x2c); // image separator
            this.WriteShort(0); // image position x,y = 0,0
            this.WriteShort(0);
            this.WriteShort(this.Width); // image size
            this.WriteShort(this.Height);

            // packed fields
            if (firstframe) {
                // no LCT  - GCT is used for first (or only) frame
                this.Stream.WriteByte(0);
            } else {
                // specify normal LCT
                this.Stream.WriteByte(Convert.ToByte(0x80 | // 1 local color table  1=yes
					0 | // 2 interlace - 0=no
					0 | // 3 sorted - 0=no
					0 | // 4-5 reserved
					this.PaletteSize)); // 6-8 size of color table
            }
        }

        private void WritePixels(byte[] pixelindexes) {
            LZWEncoder encoder = new LZWEncoder(this.Width, this.Height, pixelindexes, this.ColorDept);
            encoder.Encode(this.Stream);
        }

        private void WriteShort(int value) {
            this.Stream.WriteByte(Convert.ToByte(value & 0xff));
            this.Stream.WriteByte(Convert.ToByte((value >> 8) & 0xff));
        }

        private void WriteString(String s) {
            char[] chars = s.ToCharArray();
            for (int i = 0; i < chars.Length; i++) {
                this.Stream.WriteByte((byte)chars[i]);
            }
        }
    }
}

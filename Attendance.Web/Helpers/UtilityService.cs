using Attendance.ClientModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Net;
using System.Web;

namespace Attendance.Web.Helpers
{
    public static class UtilityService
    {
        public static string GenerateOTP()
        {
            string numbers = "1234567890";

            string characters = numbers;

            int length = 6;
            string otp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character, StringComparison.Ordinal) != -1);
                otp += character;
            }

            return otp;
        }

        //Numeric int to string
        public static string GetNumericStringFromIntegerValue(int intValue, int length)
        {
            try
            {
                return new string((intValue).ToString().ToArray()).PadLeft(length, '0');
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //Numeric string to int
        public static int GetIntegerValueFromNumericString(string numeric)
        {
            try
            {
                return int.Parse(numeric, System.Globalization.NumberStyles.Integer);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public static List<BreadcrumbModel> GenerateBreadCrumbs(List<BreadcrumbModel> items)
        {
            var breadCrumbs = new List<BreadcrumbModel>();
            breadCrumbs.Add(new BreadcrumbModel
            {
                Name = "Home",
                Link = "/User/Dashboard"
            });
            foreach (var item in items)
            {
                breadCrumbs.Add(new BreadcrumbModel
                {
                    Name = item.Name,
                    Link = item.Link
                });
            }

            return breadCrumbs;
        }

        public static string GetRootUrl()
        {
            Uri tmpURi = HttpContext.Current.Request.Url;
            var tmpPort = (tmpURi.Port > 0 && tmpURi.Port != 80) ? ":" + tmpURi.Port : "";
            var rootUrl = tmpURi.Scheme + "://" + tmpURi.Host + tmpPort + "/";
            return rootUrl;
        }

        public static bool URLExists(string url)
        {
            bool result = true;

            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Timeout = 1200; // miliseconds
            webRequest.Method = "HEAD";

            try
            {
                webRequest.GetResponse();
            }
            catch
            {
                result = false;
            }

            return result;
        }


        /// <summary>
        /// Creates an image containing the given text.
        /// NOTE: the image should be disposed after use.
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="fontOptional">Font to use, defaults to Control.DefaultFont</param>
        /// <param name="textColorOptional">Text color, defaults to Black</param>
        /// <param name="backColorOptional">Background color, defaults to white</param>
        /// <param name="minSizeOptional">Minimum image size, defaults the size required to display the text</param>
        /// <returns>The image containing the text, which should be disposed after use</returns>
        public static Image DrawText(string text, Font fontOptional = null, Color? textColorOptional = null, Color? backColorOptional = null, Size? minSizeOptional = null)
        {
            Font font = SystemFonts.DefaultFont;
            if (fontOptional != null)
                font = fontOptional;

            Color textColor = Color.Black;
            if (textColorOptional != null)
                textColor = (Color)textColorOptional;

            Color backColor = Color.White;
            if (backColorOptional != null)
                backColor = (Color)backColorOptional;

            Size minSize = Size.Empty;
            if (minSizeOptional != null)
                minSize = (Size)minSizeOptional;

            //first, create a dummy bitmap just to get a graphics object
            SizeF textSize;
            using (Image img = new Bitmap(1, 1))
            {
                using (Graphics drawing = Graphics.FromImage(img))
                {
                    //measure the string to see how big the image needs to be
                    textSize = drawing.MeasureString(text, font);
                    if (!minSize.IsEmpty)
                    {
                        textSize.Width = textSize.Width > minSize.Width ? textSize.Width : minSize.Width;
                        textSize.Height = textSize.Height > minSize.Height ? textSize.Height : minSize.Height;
                    }
                }
            }

            //create a new image of the right size
            Image retImg = new Bitmap((int)textSize.Width, (int)textSize.Height);
            using (var drawing = Graphics.FromImage(retImg))
            {
                //paint the background
                drawing.Clear(backColor);

                //create a brush for the text
                using (Brush textBrush = new SolidBrush(textColor))
                {
                    drawing.DrawString(text, font, textBrush, 0, 0);
                    drawing.Save();
                }
            }
            return retImg;
        }


        /// <summary>
        /// Converting text to image (png).
        /// </summary>
        /// <param name="text">text to convert</param>
        /// <param name="font">Font to use</param>
        /// <param name="textColor">text color</param>
        /// <param name="maxWidth">max width of the image</param>
        /// <param name="path">path to save the image</param>
        public static void DrawTextImage(String text, Font font, Color textColor, int maxWidth, String path)
        {
            //first, create a dummy bitmap just to get a graphics object
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);
            //measure the string to see how big the image needs to be
            Size minSize = Size.Empty;
            minSize.Width = maxWidth;
            minSize.Height = maxWidth;
            SizeF textSize = drawing.MeasureString(text, font);
            textSize.Width = minSize.Width;
            textSize.Height = minSize.Height;
            font= new Font(SystemFonts.DefaultFont.FontFamily, SystemFonts.DefaultFont.Size*5f);

            //set the stringformat flags to rtl
            StringFormat sf = new StringFormat();
            //uncomment the next line for right to left languages
            //sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            sf.Trimming = StringTrimming.Word;
            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();
            int padding = 50;
            //create a new image of the right size
            img = new Bitmap((int) textSize.Width + padding, (int) textSize.Height + padding);

            drawing = Graphics.FromImage(img);
            //Adjust for high quality
            drawing.CompositingQuality = CompositingQuality.HighQuality;
            drawing.InterpolationMode = InterpolationMode.HighQualityBilinear;
            drawing.PixelOffsetMode = PixelOffsetMode.HighQuality;
            drawing.SmoothingMode = SmoothingMode.HighQuality;
            drawing.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            //paint the background
            drawing.Clear(Color.Black);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);
            //drawing.DrawString(text, font, textBrush, new RectangleF(0, 0, textSize.Width, textSize.Height), sf);
            drawing.DrawString(text, font, textBrush, padding/2, padding/2);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();
            img.Save(path, ImageFormat.Png);
            img.Dispose();

        }
    }
}
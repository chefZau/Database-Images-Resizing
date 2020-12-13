// Reference: https://stackoverflow.com/questions/55162000/how-can-i-convert-a-bmp-to-jpeg-without-saving-to-file

using System;
using System.Runtime;
using System.IO;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Drawing;
using System.IO.Compression;
using System.Drawing.Imaging;

public partial class StoredProcedures
{
    private static ImageCodecInfo GetEncoder(ImageFormat format)
    {
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

        foreach (ImageCodecInfo codec in codecs)
        {
            if (codec.FormatID == format.Guid)
            {
                return codec;
            }
        }
        return null;
    }

    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void ResizeImage(out Byte[] value, Byte[] img, int width, int height)
    {   
        // Store Byte array to stream, and convert it to an Image Object
        MemoryStream myMemStream = new MemoryStream(img);
        Image imgToResize = Image.FromStream(myMemStream);

        // Calculate the new width and height under aspect ratio
        width = Math.Min(width, imgToResize.Width);
        height = Math.Min(height, imgToResize.Height);

        decimal rnd = Math.Min(width / (decimal) imgToResize.Width, height / (decimal) imgToResize.Height);
        int newWidth = (int) Math.Round(imgToResize.Width * rnd);
        int newHeight = (int) Math.Round(imgToResize.Height * rnd);

        // Apply the new size and get the bitmap version
        Size size = new Size(newWidth, newHeight);
        Bitmap bmp = new Bitmap(imgToResize, size);

        // Convert the bitmap the JEPG format, while setting the quality of image 50L
        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
        EncoderParameters myEncoderParameters = new EncoderParameters(1);
        EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
        myEncoderParameters.Param[0] = myEncoderParameter;

        // Apply the JPEG format
        MemoryStream stream = new MemoryStream();
        bmp.Save(stream, jpgEncoder, myEncoderParameters);

        value = stream.ToArray();
    }
}

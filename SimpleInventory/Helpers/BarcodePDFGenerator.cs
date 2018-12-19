﻿using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SimpleInventory.Helpers
{
    // class for generating a PDF of X number of barcodes via PDFSharp and BarcodeLib
    class BarcodePDFGenerator
    {
        // http://james-ramsden.com/c-convert-image-bitmapimage/
        private BitmapImage ConvertImageToBitmapImage(Image img)
        {
            using (var memory = new MemoryStream())
            {
                img.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public void GenerateBarcodes(string outputPath, uint numberOfPages = 1)
        {
            if (numberOfPages > 0)
            {
                PdfDocument document = new PdfDocument();
                document.Info.Title = "SimpleInventory Barcodes";
                for (int i = 0; i < numberOfPages; i++)
                {
                    PdfPage page = document.AddPage();
                    page.Size = PdfSharp.PageSize.A4; // TODO: allow for A4 or 8.5x11

                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                    XUnit yCoord = XUnit.FromInch(1); // pixels
                    gfx.DrawString("SimpleInventory Barcodes", font, XBrushes.Black,
                        new XRect(0, yCoord, page.Width, page.Height), XStringFormats.TopCenter);

                    yCoord += XUnit.FromInch(0.7);

                    // Generate a barcode
                    var barcodeCreator = new BarcodeLib.Barcode();
                    barcodeCreator.ImageFormat = ImageFormat.Jpeg;
                    barcodeCreator.IncludeLabel = true;
                    barcodeCreator.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;
                    barcodeCreator.Alignment = BarcodeLib.AlignmentPositions.CENTER;

                    bool isPageFull = false;
                    XUnit imageHeight = XUnit.FromPoint(60);
                    while (!isPageFull)
                    {
                        var isWidthFull = false;
                        XUnit xCoord = XUnit.FromInch(1);
                        while (!isWidthFull)
                        {
                            var image = barcodeCreator.Encode(BarcodeLib.TYPE.CODE128, "00000001");
                            if (image != null)
                            {
                                XImage pdfImage = XImage.FromBitmapSource(ConvertImageToBitmapImage(image));
                                gfx.DrawImage(pdfImage, xCoord, yCoord);
                                xCoord += XUnit.FromPoint(pdfImage.PointWidth);
                                imageHeight = XUnit.FromPoint(pdfImage.PointHeight);
                                var blah = XUnit.FromPoint(image.Width);
                                if (xCoord + XUnit.FromPoint(pdfImage.PointWidth) > page.Width - XUnit.FromInch(1))
                                {
                                    isWidthFull = true;
                                }
                            }
                            else
                            {
                                // failure case
                                isWidthFull = true;
                                isPageFull = true;
                                break;
                            }
                        }
                        yCoord += imageHeight;
                        yCoord += XUnit.FromInch(0.7);
                        if (yCoord + imageHeight > page.Height - XUnit.FromInch(1))
                        {
                            isPageFull = true;
                        }
                    }
                }
                // save the document and start the process for viewing the pdf
                document.Save(outputPath);
                Process.Start(outputPath);
            }
        }
    }
}

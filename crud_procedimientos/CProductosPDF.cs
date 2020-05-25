using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using iTextSharp.text;
using iTextSharp.text.pdf;


namespace crud_procedimientos
{
    class CProductosPDF
    {
        private const float milimetro = 2.83465f;

        public void Imprimir()
        { 
            Document document = new Document();    // Documento PDF.

            document.SetPageSize(iTextSharp.text.PageSize.A4); // Tamaño de la hoja A$
            document.SetMargins(10f * milimetro,               // Margen derecho 10 mm.
                                10f * milimetro,               // Margen izquierdo 10 mm. 
                                30f * milimetro,               // Margen superior 30 mm. 
                                10f * milimetro);              // Margen inferior 10 mm.

            try
            {
                // Guardaremos el contenido en un fichero "test.pdf"
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, new FileStream("test.pdf", FileMode.Create));

                pdfWriter.PageEvent = new HeaderFooter();  // Indicamos nuestro pie de página personalizado.
            }
            catch(Exception ex)
            {
                // En caso de nos poder guardar en "test.pdf" mensaje de error y adiós.
                MessageBox.Show("Al intentar abrir \"test.pdf\"." + ex.Message, "PDF", MessageBoxButtons.OK, MessageBoxIcon.Error); 

                return;
            }

            document.Open(); // Abrimos el documentos.

            try
            {
                CProductosBD productos = new CProductosBD(); // Creamos el objeto productos.

                DataRow[] rows = productos.Seleccionar().Select(); // Seleccionamos todos los productos.

                PdfPTable tProductos = new PdfPTable(5);  // Tabla para mostrar los productos.
                PdfPCell celdaCabecera = new PdfPCell();  // Celda para la cabecera. 
                PdfPCell celdaDerecha = new PdfPCell();   // Celda para alineación derecha.

                tProductos.WidthPercentage = 100f; // La tabla tendrá una anchura del 100%

                celdaDerecha.HorizontalAlignment = Element.ALIGN_RIGHT; // La celda derecha pues eso a la derecha.
                celdaDerecha.FixedHeight = 7 * milimetro;

                celdaCabecera.HorizontalAlignment = Element.ALIGN_CENTER;       // Las celdas de la cabecera se alinean al centro,
                celdaCabecera.BackgroundColor = new BaseColor(Color.LightGray); // con el fondo en gris claro
                celdaCabecera.FixedHeight = 7 * milimetro;                      // y una altura de celda de 7 mm.


                celdaCabecera.Phrase = new Phrase("Código");   // Título de la celda código.
                tProductos.AddCell(celdaCabecera);             // Añadimos la celda.
                
                celdaCabecera.Phrase = new Phrase("Producto");
                tProductos.AddCell(celdaCabecera);
                
                celdaCabecera.Phrase = new Phrase("Categoría");
                tProductos.AddCell(celdaCabecera);
                
                celdaCabecera.Phrase = new Phrase("Marca");
                tProductos.AddCell(celdaCabecera);
                
                celdaCabecera.Phrase = new Phrase("Precio");
                tProductos.AddCell(celdaCabecera);

                tProductos.HeaderRows = 1; // La primera fila, la cabecear, se repetirá en cada nueva página.

                // Para cada una de las filas de nuestra consulta...
                for (int i = 0; i<rows.Count(); i++)
                {
                    celdaDerecha.Phrase = new Phrase(rows[i]["Código"].ToString());  // Código. 
                    tProductos.AddCell(celdaDerecha);                                // Observar que no lo insertamos directamente porque lo queremos alinear a la derecha. 

                    tProductos.AddCell(rows[i]["Producto"].ToString());  // Producto.
                    tProductos.AddCell(rows[i]["Categoría"].ToString()); // Categoría.
                    tProductos.AddCell(rows[i]["Marca"].ToString());     // Marca.

                    celdaDerecha.Phrase = new Phrase(rows[i]["Precio"].ToString() + " €"); // Precio.
                    tProductos.AddCell(celdaDerecha);
                }

                document.Add(tProductos);  // Añadimos la tabla al documento.
            }
            catch (Exception ex)
            {
                // En caso de error indicamos que error se ha producido.
                MessageBox.Show("Al generar el PDF.\n\n" + ex.Message, "PDF", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                document.Close(); // y lo cerramos.
            }

            MessageBox.Show("PDF creado correctamente.", "PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    class HeaderFooter : PdfPageEventHelper
    {
        private const float milimetro = 2.83465f; // 1 milímetro equicala a 2.83465 puntos.

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            PdfPTable tCabecera = new PdfPTable(3);  // Tabla para poner la información en la cabecera.
            PdfPTable tPie = new PdfPTable(3);       // Tabla para poner la información en el pie.

            PdfPCell celdaLogo = new PdfPCell();       // Celda para poner el logo.
            PdfPCell celdaTitulo = new PdfPCell();     // Celda para pone el título.
            PdfPCell celdaFecha = new PdfPCell();      // Celda para poner la fecha. 
            PdfPCell celdaNumPagina = new PdfPCell();  // Celda para poner el número de página

            iTextSharp.text.Font fontTitulo = FontFactory.GetFont("Arial", 18f); // Fuente para utilizar en el título.
            iTextSharp.text.Font fontPie = FontFactory.GetFont("Arial", 10f);    // Fuente para utilizar en el pie.

            fontTitulo.Color = new BaseColor(0, 71, 185);   // Color y
            fontTitulo.SetStyle(iTextSharp.text.Font.BOLD); // estilo.

            fontPie.Color = fontTitulo.Color;               // Color y
            fontPie.SetStyle(iTextSharp.text.Font.NORMAL);  // estilo.

            celdaTitulo.VerticalAlignment = Element.ALIGN_MIDDLE; // Alineación vertical de la celda.
            celdaLogo.Border = 0;                                 // Sin borde. 

            celdaTitulo.VerticalAlignment = Element.ALIGN_BOTTOM;
            celdaTitulo.HorizontalAlignment = Element.ALIGN_RIGHT; // Alineación horizontal de la celda.
            celdaTitulo.Border = 0;

            celdaFecha.HorizontalAlignment = Element.ALIGN_LEFT;
            celdaFecha.Border = 0;

            celdaNumPagina.HorizontalAlignment = Element.ALIGN_RIGHT;
            celdaNumPagina.Border = 0;

            tCabecera.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin; // El tamaño de la tabla para poner la cabecera es del 100%
            tCabecera.DefaultCell.Border = 0;

            string url = "https://www.igformacion.com/wp-content/uploads/2019/04/logo-igformacion.png"; // Dirección del logo.

            celdaLogo.Image = iTextSharp.text.Image.GetInstance(new Uri(url));   // Cargamos la imagen desde la dirección indicada en la celda.
            celdaTitulo.Phrase = new Phrase("Listado de productos", fontTitulo); // Ponemos el título del listado.

            tCabecera.AddCell(celdaLogo);   // Añadimos el logo a la tabla cabecera.
            tCabecera.AddCell("");
            tCabecera.AddCell(celdaTitulo); // Añadimos el título a la tabla cabecera.

            // Escribimos la tabla cabecera
            tCabecera.WriteSelectedRows(0,                                                              // Desde la posición 0.
                                       -1,                                                              // Toda la tabla (Sólo hay una fila)
                                        document.LeftMargin,                                            // Left -> Margen izquierda.
                                        writer.PageSize.GetTop(document.TopMargin) + (25f * milimetro), // Top -> Márgen superior más 25 mm.
                                        writer.DirectContent);                                          // Dirección. 

            tPie.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            tPie.DefaultCell.Border = 0;

            celdaFecha.Phrase = new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), fontPie); // Fecha de impresión.
            celdaNumPagina.Phrase = new Phrase("Página " + writer.PageNumber, fontPie);   // Número de página.

            tPie.AddCell(celdaFecha);
            tPie.AddCell("");
            tPie.AddCell(celdaNumPagina);

            tPie.WriteSelectedRows(0, -1, document.LeftMargin, writer.PageSize.GetBottom(document.BottomMargin), writer.DirectContent);
        }
    }

}

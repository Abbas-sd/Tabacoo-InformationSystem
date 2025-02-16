using System;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TobaccoStore
{
    public partial class testing_first_print : Form
    {
        private string connectionString = "Server=MSI\\SQLEXPRESS;Database=TabacooStore;Trusted_Connection=True;";
        public testing_first_print()
        {
            InitializeComponent();
        }
        private DataTable invoiceTable = new DataTable();
        private int invoiceIdToPrint;

        private void LoadInvoiceData(int invoiceId)
        {
            invoiceIdToPrint = invoiceId;
            string query = @"
        SELECT 
            co.customer_order_id AS InvoiceNumber,
            c.fname + ' ' + c.lname AS CustomerName,
            c.phone AS CustomerPhone,
            co.order_date AS OrderDate,
            p.product_name AS ProductName,
            cod.quantity AS Quantity,
            cod.selling_price_at_order AS UnitPrice,
            (cod.quantity * cod.selling_price_at_order) AS TotalPrice,
            co.total_amount AS GrandTotal
        FROM CustomerOrder co
        JOIN Customer c ON co.customer_id = c.customer_id
        JOIN CustomerOrderDetails cod ON co.customer_order_id = cod.customer_order_id
        JOIN Product p ON cod.product_id = p.product_id
        WHERE co.customer_order_id = @InvoiceID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);
                invoiceTable.Clear();
                adapter.Fill(invoiceTable);
            }
        }


        private void PrintInvoice(object sender, PrintPageEventArgs e)
        {
            if (invoiceTable.Rows.Count == 0)
                return;

            Font headerFont = new Font("Arial", 14, FontStyle.Bold);
            Font subHeaderFont = new Font("Arial", 12, FontStyle.Bold);
            Font bodyFont = new Font("Arial", 10);
            Font boldFont = new Font("Arial", 10, FontStyle.Bold);
            Brush brush = Brushes.Black;

            float yPosition = 20;
            float leftMargin = e.MarginBounds.Left;
            float rightMargin = e.MarginBounds.Right;

            DataRow invoice = invoiceTable.Rows[0];

            // Header
            e.Graphics.DrawString("INVOICE", headerFont, brush, leftMargin + 200, yPosition);
            yPosition += 40;

            e.Graphics.DrawString($"Invoice No: {invoice["InvoiceNumber"]}", subHeaderFont, brush, leftMargin, yPosition);
            yPosition += 20;
            e.Graphics.DrawString($"Customer: {invoice["CustomerName"]}", bodyFont, brush, leftMargin, yPosition);
            yPosition += 20;
            e.Graphics.DrawString($"Phone: {invoice["CustomerPhone"]}", bodyFont, brush, leftMargin, yPosition);
            yPosition += 20;
            e.Graphics.DrawString($"Date: {Convert.ToDateTime(invoice["OrderDate"]).ToString("yyyy-MM-dd")}", bodyFont, brush, leftMargin, yPosition);
            yPosition += 40;

            // Table Header
            e.Graphics.DrawString("Product", boldFont, brush, leftMargin, yPosition);
            e.Graphics.DrawString("Qty", boldFont, brush, leftMargin + 250, yPosition);
            e.Graphics.DrawString("Price", boldFont, brush, leftMargin + 300, yPosition);
            e.Graphics.DrawString("Total", boldFont, brush, leftMargin + 400, yPosition);
            yPosition += 20;
            e.Graphics.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
            yPosition += 10;

            // Print Items
            foreach (DataRow row in invoiceTable.Rows)
            {
                e.Graphics.DrawString(row["ProductName"].ToString(), bodyFont, brush, leftMargin, yPosition);
                e.Graphics.DrawString(row["Quantity"].ToString(), bodyFont, brush, leftMargin + 250, yPosition);
                e.Graphics.DrawString(row["UnitPrice"].ToString(), bodyFont, brush, leftMargin + 300, yPosition);
                e.Graphics.DrawString(row["TotalPrice"].ToString(), bodyFont, brush, leftMargin + 400, yPosition);
                yPosition += 20;
            }

            yPosition += 10;
            e.Graphics.DrawLine(Pens.Black, leftMargin, yPosition, rightMargin, yPosition);
            yPosition += 20;

            // Grand Total
            e.Graphics.DrawString($"Grand Total: {invoice["GrandTotal"]}", subHeaderFont, brush, leftMargin + 300, yPosition);
        }

        private void testing_first_print_Load(object sender, EventArgs e)
        {

        }

        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            int invoiceId = 1; // Use the correct invoice ID based on your logic
            LoadInvoiceData(invoiceId);

            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(PrintInvoice);

            PrintPreviewDialog previewDialog = new PrintPreviewDialog
            {
                Document = printDocument,
                Width = 800,
                Height = 600
            };

            previewDialog.ShowDialog();
        }


        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += new PrintPageEventHandler(PrintInvoice);
            printDocument.Print();
        }
    }
}

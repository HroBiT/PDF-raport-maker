using System;
using System.IO;
using System.Linq;
using System.Windows;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text;
            DateTime? reportDate = ReportDatePicker.SelectedDate;
            string[] dataLines = DataTextBox.Text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (string.IsNullOrWhiteSpace(title) || !reportDate.HasValue || !dataLines.Any())
            {
                MessageBox.Show("Uzupełnij wszystkie pola przed eksportem.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "raport.pdf");

            var reportData = new
            {
                Title = title,
                Date = reportDate.Value.ToString("yyyy-MM-dd"),
                Items = dataLines
            };

            var document = CreateDocument(reportData);
            document.GeneratePdf(filePath);

            MessageBox.Show($"Raport zapisano w: {filePath}", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private IDocument CreateDocument(dynamic data)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.Content()
                        .Column(column =>
                        {
                            column.Item().Text(data.Title).FontSize(20).Bold();
                            column.Item().Text($"Data: {data.Date}").FontSize(14);
                            column.Item().PaddingVertical(10).Element(CreateTable(data.Items));
                        });
                });
            });
        }

        private Action<IContainer> CreateTable(string[] items)
        {
            return container =>
            {
                container.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Nr").Bold();
                        header.Cell().Text("Opis").Bold();
                    });

                    for (int i = 0; i < items.Length; i++)
                    {
                        table.Cell().Text((i + 1).ToString());
                        table.Cell().Text(items[i]);
                    }
                });
            };
        }
    }
}

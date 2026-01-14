using ComplaignManagementSystem.Data.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.PDFHanlder
{
    public class PDFService : IDocument
    {
        private readonly Complaint_ManageProcessModel _model;

        public PDFService(Complaint_ManageProcessModel model)
        {
            _model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(25);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Text("Complaint Management System - Detail Report")
                    .FontSize(18)
                    .Bold()
                    .AlignCenter();

                page.Content().Column(col =>
                {
                    SectionTitle(col, "Complaint Detail");
                    KeyValue(col, "Reference", _model.Refference);
                    KeyValue(col, "Mode", _model.ComplaintMethod);
                    KeyValue(col, "Branch", _model.Branch);
                    KeyValue(col, "Department", _model.Dep);
                    KeyValue(col, "Nature", _model.Nature);
                    KeyValue(col, "Priority", _model.Priority);
                    KeyValue(col, "Created Date", _model.CreatedDate.ToString("dd/MM/yyyy"));

                    col.Item().PaddingVertical(10).LineHorizontal(1);

                    SectionTitle(col, "Customer Detail");
                    OptionalKeyValue(col, "Customer Name", _model.Cus_Name);
                    OptionalKeyValue(col, "NIC", _model.Cus_Nic);
                    OptionalKeyValue(col, "Reference", _model.Cus_Refference);
                    OptionalKeyValue(col, "Mobile", _model.Cus_MobileNumber);

                    col.Item().PaddingVertical(10).LineHorizontal(1);

                    SectionTitle(col, "Resolved Detail");
                    KeyValue(col, "Resolved User", _model.ResolvedUser);
                    KeyValue(col, "Remark", _model.ResolvedRemark);
                    KeyValue(col, "Resolved Date", _model.ResolvedDateTime?.ToString("dd/MM/yyyy"));

                    col.Item().PaddingVertical(10).LineHorizontal(1);

                    SectionTitle(col, "Customer Notified Detail");
                    KeyValue(col, "Notification Method", _model.CusNotification);
                    OptionalKeyValue(col, "Notification Remark", _model.CusNotificationRemark);
                    KeyValue(col, "Notified Date", _model.CusNotifiedDate?.ToString("dd/MM/yyyy"));
                });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated on ");
                        x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).Bold();
                    });
            });
        }

        void SectionTitle(ColumnDescriptor col, string title)
        {
            col.Item().PaddingTop(10).Text(title).Bold().FontSize(13);
        }

        void KeyValue(ColumnDescriptor col, string key, string value)
        {
            col.Item().Row(row =>
            {
                row.RelativeItem(3).Text(key).Bold();
                row.RelativeItem(7).Text(value ?? "-");
            });
        }

        void OptionalKeyValue(ColumnDescriptor col, string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                KeyValue(col, key, value);
        }
    }
}

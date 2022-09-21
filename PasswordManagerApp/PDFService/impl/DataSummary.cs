using Data.DataAccess;
using Data.Models;
using PasswordEncryption.Contracts;
using PDFService.contracts;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFService.impl
{
    public class DataSummary : IDataSummary
    {
        private readonly IUserDataRepository _userData;
        private readonly ISymmetricEncryptDecrypt _ecryptionService;

        public DataSummary(IUserDataRepository userData,ISymmetricEncryptDecrypt ecryptionService)
        {
            _userData = userData;
            _ecryptionService = ecryptionService;
        }

        public async Task<IEnumerable<byte[]>> GenerateImageSummary(ApplicationUser user, string decryptionKey)
        {
            var document = await GenerateSummaryDocument(user.Id,decryptionKey);

            return document.GenerateImages();
        }

        public async Task<byte[]> GeneratePDFSummary(ApplicationUser user, string decryptionKey)
        {
            var document = await GenerateSummaryDocument(user.Id, decryptionKey);

            return document.GeneratePdf();
        }

        public async Task<byte[]> GenerateTextFileSummary(ApplicationUser user,string decryptionKey)
        {
            var userData = await _userData.GetDataByUserId(user.Id);
            if (userData is null)
                throw new Exception("Could not find user Data");

            string textContent = String.Format("Recovery Key: XXXXXXXXX\n");
            textContent += "**********************\n";

            foreach (var account in userData.AccountInfos!)
            {
                var decryptedPassword = _ecryptionService.Decrypt(account.EncryptedPassword, account.EncryptedPasswordIV, decryptionKey);
                textContent += String.Format("Name: {2} \nEmail or Username: {0}  \nPassword: {1}\n",account.Email,decryptedPassword,account.Name);
                textContent += "**********************\n";
            }

           return Encoding.ASCII.GetBytes(textContent);
        }

        private async Task<Document> GenerateSummaryDocument(Guid userId, string decryptionKey)
        {
            var userData = await _userData.GetDataByUserId(userId);
            if (userData is null)
                throw new Exception("Could not find user Data");

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Header()
                        .Text("Hello PDF!")
                        .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(20);

                            x.Item().Text(Placeholders.LoremIpsum());
                            x.Item().Image(Placeholders.Image(200, 100));
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            });

        }
    }
}

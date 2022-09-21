using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFService.contracts
{
    public interface IDataSummary
    {
        public Task<byte[]> GeneratePDFSummary(ApplicationUser user, string decryptionKey);
        public Task<IEnumerable<byte[]>> GenerateImageSummary(ApplicationUser user, string decryptionKey);
        public Task<byte[]> GenerateTextFileSummary(ApplicationUser user, string decryptionKey);
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class File
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FileTitle { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FilePath { get; set; }

        [Required]
        public long FileSize { get; set; }

        [Required]
        public DateTime UploadDateTime { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletionDateTime { get; set; }
    }
}

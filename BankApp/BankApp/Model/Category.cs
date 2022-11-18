using PRBD_Framework;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BankApp.Model
{
    public class Category : EntityBase<BankContext>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Category() : base()
        {
        }

        public Category(string name)
        {
            Name = name;
        }

        [InverseProperty(nameof(BankTransfer.Category))]
        public virtual ICollection<BankTransfer> BankTransfers { get; set; } = new HashSet<BankTransfer>();

        public static List<Category> GetAll()
        {
            var query = Context.Categories.OrderBy(cat=>cat.Name).ToList();
            return query;
        }

        public override bool Equals(object obj)
        {
            return Id == (obj as Category)?.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}

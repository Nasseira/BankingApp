using PRBD_Framework;

using System.Collections.Generic;
using System.Linq;

namespace BankApp.Model
{
    public class Agency : EntityBase<BankContext>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Agency() : base()
        {
        }

        public Agency(string name)
        {
            Name = name;
        }

        public virtual Manager Manager { get; set; }
        public virtual ICollection<Client> Clients { get; set; } = new HashSet<Client>();

        public static IQueryable<Agency> GetAllByManager(int ManagerId)
        {
            return Context.Agencies.Where(a => a.Manager.Id == ManagerId).OrderBy(a => a.Name);
        }

        public static IQueryable<Client> GetClientByAgency(Agency Agency)
        {
            return Context.Clients.Where(c => c.Agency.Name == Agency.Name);
        }
    }
}

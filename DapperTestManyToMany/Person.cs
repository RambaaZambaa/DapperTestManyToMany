using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace DapperTestManyToMany
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Chor> Chöre { get; set; } = new List<Chor>();
        public List<Person> Eltern { get; set; } = new List<Person>();

        /// <summary>
        /// Queries all Person, including their Choirs and Parents
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Person> QueryAll()
        {
            string query = @"SELECT p.Id, p.Name, t.Id, t.Name
                     FROM Person p LEFT JOIN Person_Chor tp ON tp.Person = p.Id
                     LEFT JOIN Chor t ON tp.Chor = t.Id";
            var result = default(IEnumerable<Person>);
            Dictionary<int, Person> lookup = new Dictionary<int, Person>();
            using (IDbConnection connection = SQLHelper.CreateConnection())
            {
                result = connection.Query<Person, Chor, Person>(query, (p, t) =>
                {
                    if (!lookup.TryGetValue(p.Id, out Person personFound))
                    {
                        lookup.Add(p.Id, p);
                        personFound = p;
                    }
                    personFound.Chöre.Add(t);
                    return personFound;

                }).Distinct();
                var Eltern = connection.Query<dynamic>("SELECT Person, Elternteil FROM Eltern");
                foreach (var item in Eltern)
                {
                    if (lookup.TryGetValue(item.Person, out Person personFound))
                    {
                        if (lookup.TryGetValue(item.Elternteil, out Person elternteilFound))
                        {
                            personFound.Eltern.Add(elternteilFound);
                        }
                    }
                }
            }
            
            return result;
        }

    }
}

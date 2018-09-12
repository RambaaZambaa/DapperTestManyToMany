using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DapperTestManyToMany.Experiments
{
    public class DapperBlub
    {
        public List<Person> querymanual()
        {
            SqlConnection con = new SqlConnection("Data Source=juslap\\sqlexpress;Initial Catalog=JsonTestDB;Integrated Security=True");
            con.Open();

            List<Person> persons = new List<Person>();
            Dictionary<int, Person> PersonDict = new Dictionary<int, Person>();
            Dictionary<int, Chor> ChorDict = new Dictionary<int, Chor>();

            string query = "SELECT Id, Name from Person";
            SqlCommand myCommand = new SqlCommand(query, con);
            SqlDataReader myReader = myCommand.ExecuteReader();
            while (myReader.Read())
            {
                Person P = new Person() { Id = (int)myReader[0], Name = (string)myReader[1] };
                persons.Add(P);
                PersonDict.Add(P.Id, P);
            }
            myReader.Close();
            query = "SELECT Id, Name from Chor";
            myCommand = new SqlCommand(query, con);
            myReader = myCommand.ExecuteReader();
            while (myReader.Read())
            {
                ChorDict.Add((int)myReader[0], new Chor() { Id = (int)myReader[0], Name = (string)myReader[1] });
            }
            myReader.Close();
            query = "SELECT Person, Chor from Person_Chor";
            myCommand = new SqlCommand(query, con);
            myReader = myCommand.ExecuteReader();
            while (myReader.Read())
            {
                if (PersonDict.TryGetValue((int)myReader[0], out Person p))
                    if (ChorDict.TryGetValue((int)myReader[1], out Chor c))
                        p.Chöre.Add(c);
            }

            con.Close();
            con.Dispose();
            return persons;
        }
        public IEnumerable<Person> SelectPersons()
        {
            string query = @"SELECT p.Id, p.Name, t.Id, t.Name
                     FROM Person p INNER JOIN Person_Chor tp ON tp.Person = p.Id
                     INNER JOIN Chor t ON tp.Chor = t.Id";
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

            }
            return result;
        }

        public IEnumerable<Person> GetPersonList()
        {
            string query = @"SELECT p.Id, p.Name, t.Id, t.Name
                             FROM Person p INNER JOIN Person_Chor tp ON tp.Person = p.Id
                             INNER JOIN Chor t ON tp.Chor = t.Id";
            return SQLHelper.CreateConnection().Query<Person, Chor, Person>(query, (person, chor) => {
                person.Chöre.Add(chor);
                return person;
            }
        ,
        splitOn: "Name"
    )
    .GroupBy(o => o.Id)
    .Select(group =>
    {
        var combinedOwner = group.First();
        combinedOwner.Chöre = group.Select(owner => owner.Chöre.Single()).ToList();
        return combinedOwner;
    });
        }
    }
}

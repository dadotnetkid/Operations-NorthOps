using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NorthOps.Models;

namespace NorthOps.Services.Helpers
{
    public class DummyAgentSeederHelper
    {
        public List<Users> GenerateAgent()
        {
            List<Users> users = new List<Users>();
            Random random = new Random();
            for (var i = 1; i <= 21; i++)
            {
                users.Add(new Users()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = FirstName[i],
                    Email = FirstName[i]+"@test.local",
                    FirstName = FirstName[i],
                    MiddleName = LastName[random.Next(0, LastName.Count() - 1)],
                    LastName = LastName[random.Next(0, LastName.Count() - 1)],
                });
            }
            return users;
        }

        private List<string> FirstName
        {
            get
            {
                var names = new List<string>(new string[]
                {
                    "Kyle",
                    "Gina",
                    "Irene",
                    "Katie",
                    "Michael",
                    "Oscar",
                    "Ralph",
                    "Torrey",
                    "William",
                    "Bill",
                    "Daniel",
                    "Frank",
                    "Brenda",
                    "Danielle",
                    "Fiona",
                    "Howard",
                    "Jack",
                    "Larry",
                    "Holly",
                    "Jennifer",
                    "Liz",
                    "Pete",
                    "Steve",
                    "Vince",
                    "Zeke",
                    "Aiden",
                    "Jackson",
                    "Mason  ",
                    "Liam   ",
                    "Jacob  ",
                    "Jayden ",
                    "Ethan  ",
                    "Noah   ",
                    "Lucas  ",
                    "Logan  ",
                    "Caleb  ",
                    "Caden  ",
                    "Jack   ",
                    "Ryan   ",
                    "Connor ",
                    "Michael",
                    "Elijah ",
                    "Brayden",
                    "Benjamin",
                    "Nicholas",
                    "Alexander",
                    "William",
                    "Matthew",
                    "James  ",
                    "Landon ",
                    "Nathan ",
                    "Dylan  ",
                    "Evan   ",
                    "Luke   ",
                    "Andrew ",
                    "Gabriel",
                    "Gavin  ",
                    "Joshua ",
                    "Owen   ",
                    "Daniel ",
                    "Carter ",
                    "Tyler  ",
                    "Cameron",
                    "Christian",
                    "Wyatt  ",
                    "Henry  ",
                    "Eli    ",
                    "Joseph ",
                    "Max    ",
                    "Isaac  ",
                    "Samuel ",
                    "Anthony",
                    "Grayson",
                    "Zachary",
                    "David  ",
                    "Christopher",
                    "John   ",
                    "Isaiah ",
                    "Levi   ",
                    "Jonathan",
                    "Oliver ",
                    "Chase  ",
                    "Cooper ",
                    "Tristan",
                    "Colton ",
                    "Austin ",
                    "Colin  ",
                    "Charlie",
                    "Dominic",
                    "Parker ",
                    "Hunter ",
                    "Thomas ",
                    "Alex   ",
                    "Ian    ",
                    "Jordan ",
                    "Cole   ",
                    "Julian ",
                    "Aaron  ",
                    "Carson ",
                    "Miles  ",
                    "Blake  ",
                    "Brody  ",
                    "Adam   ",
                    "Sebastian",
                    "Adrian ",
                    "Nolan  ",
                    "Sean   ",
                    "Riley  ",
                    "Bentley",
                    "Xavier ",
                    "Hayden ",
                    "Jeremiah",
                    "Jason  ",
                    "Jake   ",
                    "Asher  ",
                    "Micah  ",
                    "Jace   ",
                    "Brandon",
                    "Josiah ",
                    "Hudson ",
                    "Nathaniel",
                    "Bryson ",
                    "Ryder  ",
                    "Justin ",
                    "Bryce  "
                });
                return names;
            }

        }

        public List<string> LastName
        {
            get
            {
                var names =
                    "SMITH,JOHNSON,WILLIAMS,BROWN,JONES,MILLER,DAVIS,GARCIA,RODRIGUEZ,WILSON,MARTINEZ,ANDERSON,TAYLOR,THOMAS,HERNANDEZ,MOORE,MARTIN,JACKSON,THOMPSON,WHITE,LOPEZ,LEE,GONZALEZ,HARRIS,CLARK,LEWIS,ROBINSON,WALKER,PEREZ,HALL,YOUNG,ALLEN,SANCHEZ,WRIGHT,KING,SCOTT,GREEN,BAKER,ADAMS,NELSON,HILL,RAMIREZ,CAMPBELL,MITCHELL,ROBERTS,CARTER,PHILLIPS,EVANS,TURNER,TORRES,PARKER,COLLINS,EDWARDS,STEWART,FLORES,MORRIS,NGUYEN,MURPHY,RIVERA,COOK,ROGERS,MORGAN,PETERSON,COOPER,REED,BAILEY,BELL,GOMEZ,KELLY,HOWARD,WARD,COX,DIAZ,RICHARDSON,WOOD,WATSON,BROOKS,BENNETT,GRAY,JAMES,REYES,CRUZ,HUGHES,PRICE,MYERS,LONG,FOSTER,SANDERS,ROSS,MORALES,POWELL,SULLIVAN,RUSSELL,ORTIZ,JENKINS,GUTIERREZ,PERRY,BUTLER,BARNES,FISHER,HENDERSON,COLEMAN,SIMMONS,PATTERSON,JORDAN,REYNOLDS,HAMILTON,GRAHAM,KIM,GONZALES,ALEXANDER,RAMOS,WALLACE,GRIFFIN,WEST,COLE,HAYES,CHAVEZ,GIBSON,BRYANT,ELLIS,STEVENS,MURRAY,FORD,MARSHALL,OWENS,MCDONALD,HARRISON,RUIZ,KENNEDY,WELLS,ALVAREZ,WOODS,MENDOZA,CASTILLO,OLSON,WEBB,WASHINGTON,TUCKER,FREEMAN,BURNS,HENRY,VASQUEZ,SNYDER,SIMPSON,CRAWFORD,JIMENEZ,PORTER,MASON,SHAW,GORDON,WAGNER,HUNTER,ROMERO,HICKS,DIXON,HUNT,PALMER,ROBERTSON,BLACK,HOLMES,STONE,MEYER,BOYD,MILLS,WARREN,FOX,ROSE,RICE,MORENO,SCHMIDT,PATEL,FERGUSON,NICHOLS,HERRERA,MEDINA,RYAN,FERNANDEZ,WEAVER";
                return names.Split(',').Select(x => x).ToList();


            }
        }
    }

}

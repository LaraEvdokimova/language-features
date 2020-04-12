// try     {участок кода который может выбросить ошибки}
// catch   {обработка исключений}
// finally {блок который всегда выполняется (почти всегда)}

 using System;
 using System.Collections.Generic;
 using System.Linq;
 using System.Text;
 using System.Threading.Tasks;

    namespace Aviacompany
{
        class Program
        {
            static void Main(string[] args)
            {
                try
                 {
                  int.Parse("Лара-95");
                 }
                 catch
                 {
                   Console.WriteLine();
                 }

                double totalPlaneCapacity = 0, totalPlaneCarrying = 0;
                aircompany listObject = new aircompany();
                listObject.planeList.Add(new plane() { name = "Ту-134", type = "малая", capacity = 90, carrying = 47000, range = 2800 });
                listObject.planeList.Add(new plane() { name = "ИЛ-62 ", type = "большая", capacity = 180, carrying = 280300, range = 10000 });
                listObject.planeList.Add(new plane() { name = "Airbus A310", type = "средняя и большая", capacity = 183, carrying = 164000, range = 3000 });
                listObject.planeList.Add(new plane() { name = "Боинг-737", type = "малая и средняя", capacity = 114, carrying = 52800, range = 2518 });
                listObject.planeList.Add(new plane() { name = "Боинг-777 ", type = " большая", capacity = 235, carrying = 242600, range = 7406 });
                Console.WriteLine(listObject.ToString());

                foreach (plane p in listObject.planeList)
                {
                    totalPlaneCapacity += p.capacity;
                    totalPlaneCarrying += p.carrying;
                }
            Console.WriteLine("Общая вместимость {0}:", totalPlaneCapacity);
            Console.WriteLine("Общая грузоподъемность: {0}", totalPlaneCarrying);

            Console.ReadKey();
            }
        }
        class plane
        {

            public string name;
            public string type;
            public double capacity;
            public double carrying;
            public double range;
            public override string ToString()
            {
                return String.Format("Имя: {0,11} | Протяженность: {1,17} | Вместимость: {2,10} | Грузоподъемность: {3,10} | Дальность полета: {4,10}", this.name, this.type, this.capacity, this.carrying, this.range);
            }


        }

        class aircompany
        {
            public List<plane> planeList = new List<plane>();

            public override string ToString()
            {
                StringBuilder strBuilder = new StringBuilder();
                foreach (var item in planeList)
                {
                    strBuilder.AppendLine(item.ToString());

                }

                return strBuilder.ToString();
            }

        }
    }

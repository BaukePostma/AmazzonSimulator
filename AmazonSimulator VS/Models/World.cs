using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;

namespace Models
{
    public class World : IObservable<Command>, IUpdatable
    {
        public List<Abstract_Model> worldObjects = new List<Abstract_Model>();
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();

        Robot r;
        Truck t;
        Dijkstra d;

        public World()
        {
            Dijkstra g = new Dijkstra();
            Node testnode = new Node('A',0,0,0);
            Node endnode = new Node('D',30,0,30);
           
            g.addNode(testnode, new Dictionary<char, int>() { { 'B', 7 }, { 'C', 8 } });
  
          

           // g.shortest_path('A', 'H').ForEach(x => Console.WriteLine(x));
            t = SpawnTruck(-20,0,0);
            d = new Dijkstra();
            List<char> paths = d.shortest_path('A','F');
            Rek q = CreateRek(-100,0,0);
            r = CreateRobot(0, 0, 0);

            r.PickupRek();
           // MoveModel(r, 50, 0, 0);
        }

        private Truck SpawnTruck(double x, double y, double z)
        {
            Truck t = new Truck(x, y, z, 0, 0, 0);
            worldObjects.Add(t);
            return t;
        }
        private Rek CreateRek(double x, double y, double z)
        {
            Rek rek = new Rek (x, y, z, 0, 0, 0);
            worldObjects.Add(rek);
            return rek;
        }
        private Robot CreateRobot(double x, double y, double z)
        {
            Robot constructorrobot = new Robot(x, y, z, 0, 0, 0,this);
            worldObjects.Add(constructorrobot);
            return constructorrobot;
        }
        //public void MoveModel(Abstract_Model model,double x, double y , double z)
        //{
        //    //Check if you need to move on an axis
        //    // Check if you need to move less than a 'tick'
        //    // if true, move the  last remaining bit
        //    // if false, move the tick valye
        //    // Repeat until the move is done

        //    double xdif = x - model.x;
        //    double ydif = y - model.y;
        //    double zdif = z - model.z;
        //    bool destinationreached = false;
        //    // 3 times, for each axis
        //    while (!destinationreached)
        //    {


        //        if (model.needsUpdate)
        //        {



        //            // If not 0, i need to move on the X axis
        //            if (xdif != 0)
        //            {
        //                // If less than 5 , 
        //                if (xdif < 5)
        //                {
        //                    model.Move(xdif, 0, 0);
        //                    destinationreached = true;

        //                }
        //                else
        //                {
        //                    model.Move(5, 0, 0);
        //                    xdif = xdif - 5;
        //                }
        //            }

        //            if (xdif == 0)
        //            {
        //                destinationreached = true;
        //            }


        //        }
        //    }
        //}

        public IDisposable Subscribe(IObserver<Command> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);

                SendCreationCommandsToObserver(observer);
            }
            return new Unsubscriber<Command>(observers, observer);
        }

        private void SendCommandToObservers(Command c)
        {
            for (int i = 0; i < this.observers.Count; i++)
            {
                this.observers[i].OnNext(c);
            }
        }

        private void SendCreationCommandsToObserver(IObserver<Command> obs)
        {
            foreach (Abstract_Model m3d in worldObjects)
            {
                obs.OnNext(new UpdateModel3DCommand(m3d));
            }
        }

        public bool Update(int tick)
        {
           // r.MoveTo(30, 0, 30);
           // t.MoveTo(30, 0, 0);
            for (int i = 0; i < worldObjects.Count; i++)
            {
                Abstract_Model u = worldObjects[i];

                if (u is IUpdatable)
                {
                    bool needsCommand = ((IUpdatable)u).Update(tick);
                    if (needsCommand)
                    {
                        SendCommandToObservers(new UpdateModel3DCommand(u));
                    }
                }
            }
            return true;
        }
    }

    internal class Unsubscriber<Command> : IDisposable
    {
        private List<IObserver<Command>> _observers;
        private IObserver<Command> _observer;

        internal Unsubscriber(List<IObserver<Command>> observers, IObserver<Command> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}
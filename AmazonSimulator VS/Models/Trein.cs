using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;


namespace Models
{
    public enum TreiState
    {
        TRAIN_INCOMMING,
        AT_LOADING_DOCK,
        TRAIN_DEPARTING
    }
    public class Trein : Abstract_Model
    {
        World _world;
        TreiState _state;

        Rek CarriedRek;

        public Trein(double x, double y, double z, double rotationX, double rotationY, double rotationZ, World w)
        {
            this.type = "trein";
            this.guid = Guid.NewGuid();
            this._world = w;
            this._x = x;
            this._y = y;
            this._z = z;

            this._rX = rotationX;
            this._rY = rotationY;
            this._rZ = rotationZ;

            _state = TreiState.TRAIN_INCOMMING;

        }

        

        int AtLoadingDock()
        {
            this._state = TreiState.AT_LOADING_DOCK;
            Rek cargo = this.Unload();

            this._world.TrainArrived(this, cargo);
            
            return 0;
        }

        int Departed()
        {
            // Als trein helemaal weg gereden is

            return 0;
        }

        Rek Unload()
        {
            return this._world.CreateRek(this.x, this.y, this.z);
        }

        // Word aangeroepen door een robot
        void Load(Rek cargo)
        {
            this.CarriedRek = cargo;
        }
        
        void Depart()
        {
            this._state = TreiState.TRAIN_DEPARTING;
        }

        public override bool Update(int tick)
        {

            switch (this._state)
            {
                case TreiState.TRAIN_INCOMMING:
                    this.MoveTo(15, 0, -5, this.AtLoadingDock);
                    break;
                case TreiState.AT_LOADING_DOCK:
                    if(CarriedRek != null) // Als we een rek hebben gekregen sinds de laatste Update()
                    {
                        Depart();
                    }
                    break;
                case TreiState.TRAIN_DEPARTING:
                    this.MoveTo(40, 0, -5, this.Departed);
                    break;
            }

            if(CarriedRek != null)
            {
                CarriedRek.Move(this.x, this.y, this.z);
            }
            

           
            // CreateRek(15,0,-5);

            if (needsUpdate)
            {
                needsUpdate = false;
                return true;

            }

            return false;
        }
    }
}
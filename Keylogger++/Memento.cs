using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keylogger__
{
    // Memento pattern implementation
    //TODO add to interface and actually use this
    internal class Memento
    {
        private readonly string state;
        public Memento(string state)
        {
            this.state = state;
        }

        public string GetState()
        {
            return state;
        }
    }

    class Originator
    {
        private string state;
        public string State
        {
            get { return state; }
            set { state = value; }
        }

        public Memento SaveState()
        {
            return new Memento(state);
        }

        public void RestoreState(Memento memento)
        {
            state = memento.GetState();
        }
    }

    class Caretaker
    {
        private readonly List<Memento> mementoList = new List<Memento>();

        public void AddMemento(Memento memento)
        {
            mementoList.Add(memento);
        }

        public Memento GetMemento(int index)
        {
            return mementoList[index];
        }
    }
}

using SignalR_Snake.Models.Memento;
using System.Collections.Generic;



namespace SignalR_Snake.Models.Memento
{
    public class GameCaretaker
    {
        private List<GameStateMemento> mementos = new List<GameStateMemento>();

        public void AddMemento(GameStateMemento memento)
        {
            mementos.Add(memento);
        }

        public GameStateMemento GetMemento(int index)
        {
            if (index >= 0 && index < mementos.Count)
            {
                return mementos[index];
            }
            return null;
        }

        public int GetSavedStatesCount()
        {
            return mementos.Count;
        }

        public void ClearSavedStates()
        {
            mementos.Clear();
        }
    }
}
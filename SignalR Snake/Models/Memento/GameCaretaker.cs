
namespace SignalR_Snake.Models.Memento
{
    public class SnakeHub
    {
        private GameCaretaker caretaker = new GameCaretaker();
        private bool isGameSaved;

        public void SaveGameState()
        {
            GameStateMemento memento = new GameStateMemento(snakes, foods, obstacles);
            caretaker.AddMemento(memento);
            isGameSaved = true;
        }

        public void LoadGameState(int index)
        {
            GameStateMemento memento = caretaker.GetMemento(index);
            if (memento != null)
            {
                snakes = memento.GetSnakes();
                foods = memento.GetFoods();
                obstacles = memento.GetObstacles();
                isGameSaved = false;
            }
        }

        public bool IsGameSaved() => isGameSaved;

        public int GetSavedStatesCount() => caretaker.GetSavedStatesCount();

        public void ClearAllSavedStates() => caretaker.ClearSavedStates();
    }
}
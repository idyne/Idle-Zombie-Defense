using UnityEngine;
using UnityEngine.Events;

namespace FateGames
{
    public static class PlayerProgression
    {
        public static PlayerData PlayerData { get; private set; }
        public static readonly UnityEvent<int, int> OnMoneyChanged = new();
        public static bool CanAfford(int cost) => MONEY >= cost;

        public static int CurrentLevel
        {
            get => PlayerData.CurrentLevel;
            set
            {
                PlayerData.CurrentLevel = value;
                SaveManager.Save(PlayerData);
            }
        }
        public static int MONEY
        {
            get => PlayerData.Money; set
            {
                int previous = PlayerData.Money;
                PlayerData.Money = value;
                OnMoneyChanged.Invoke(value, value - previous);
            }
        }

        public static void InitializePlayerData()
        {
            PlayerData = SaveManager.Load<PlayerData>();
            if (PlayerData == null)
            {
                PlayerData = new PlayerData();
                SaveManager.Save(PlayerData);
            }
            OnMoneyChanged.Invoke(MONEY, 0);
            Debug.Log("Player Data is initialized");
        }
    }

}

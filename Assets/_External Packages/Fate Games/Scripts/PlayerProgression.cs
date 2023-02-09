using UnityEngine;
using UnityEngine.Events;

namespace FateGames
{
    public static class PlayerProgression
    {
        public static PlayerData PlayerData { get; private set; }
        public static readonly UnityEvent<int, int> OnMoneyChanged = new();
        public static readonly UnityEvent<int, int> OnUpgradePointChanged = new();
        public static bool CanAfford(int cost) => MONEY >= cost;
        public static bool CanAffordUpgrade(int cost) => UPGRADE_POINT >= cost;

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
        public static int UPGRADE_POINT
        {
            get => PlayerData.UpgradePoint; set
            {
                int previous = PlayerData.UpgradePoint;
                PlayerData.UpgradePoint = value;
                OnUpgradePointChanged.Invoke(value, value - previous);
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
            OnUpgradePointChanged.Invoke(UPGRADE_POINT, 0);
            Debug.Log("Player Data is initialized");
        }
    }

}

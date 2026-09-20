using UnityEngine;
using UnityEngine.UI;

namespace PocketFront
{
    public class UIManager : MonoBehaviour
    {
        public Text turnText;
        public Text roundText;
        public Text blueBaseText;
        public Text redBaseText;
        public Text hintText;

        public GameObject infoPanel;
        public Text infoNameText;
        public Text infoHPText;
        public Text infoStatusText;

        public Button endTurnButton;

        public GameObject endScreen;
        public Text endTitleText;
        public Button restartButton;

        public Color playerColor = new Color(0.35f, 0.6f, 1f);
        public Color enemyColor = new Color(1f, 0.4f, 0.35f);

        void Start()
        {
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
            restartButton.onClick.AddListener(OnRestartClicked);
            endScreen.SetActive(false);
            infoPanel.SetActive(false);
        }

        void Update()
        {
            GameManager game = GameManager.Instance;
            if (game == null)
            {
                return;
            }

            ShowTurn(game);
            ShowBases(game);
            ShowSelectedUnit(game);
            ShowEndScreen(game);
        }

        void ShowTurn(GameManager game)
        {
            if (game.state == GameState.PlayerTurn)
            {
                turnText.text = "PLAYER TURN";
                turnText.color = playerColor;
                hintText.text = "Click a blue soldier, then a blue tile to move or a red tile to attack.";
            }
            else if (game.state == GameState.EnemyTurn)
            {
                turnText.text = "ENEMY TURN";
                turnText.color = enemyColor;
                hintText.text = "The enemy is moving...";
            }
            else
            {
                hintText.text = "";
            }

            roundText.text = "Round " + game.turns.turnNumber;
            endTurnButton.interactable = game.state == GameState.PlayerTurn;
        }

        void ShowBases(GameManager game)
        {
            blueBaseText.text = "BLUE BASE  HP " + game.playerBase.currentHP + " / " + game.playerBase.maxHP;
            redBaseText.text = "RED BASE  HP " + game.enemyBase.currentHP + " / " + game.enemyBase.maxHP;
        }

        void ShowSelectedUnit(GameManager game)
        {
            Unit unit = game.playerController.selected;
            if (unit == null)
            {
                infoPanel.SetActive(false);
                return;
            }

            infoPanel.SetActive(true);
            infoNameText.text = unit.unitName;
            infoHPText.text = "HP: " + unit.currentHP + " / " + unit.maxHP;

            if (unit.hasActed)
            {
                infoStatusText.text = "Status: Used";
            }
            else
            {
                infoStatusText.text = "Status: Ready";
            }
        }

        void ShowEndScreen(GameManager game)
        {
            if (game.state == GameState.Victory)
            {
                endScreen.SetActive(true);
                endTitleText.text = "VICTORY";
                endTitleText.color = playerColor;
            }
            else if (game.state == GameState.Defeat)
            {
                endScreen.SetActive(true);
                endTitleText.text = "DEFEAT";
                endTitleText.color = enemyColor;
            }
            else
            {
                endScreen.SetActive(false);
            }
        }

        void OnEndTurnClicked()
        {
            GameManager.Instance.turns.EndPlayerTurn();
        }

        void OnRestartClicked()
        {
            GameManager.Instance.Restart();
        }
    }
}

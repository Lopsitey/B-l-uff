using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Card.Fsm.States;
using Template.Content.Scripts.Managers;
using Template.UI.Views;
using UnityEngine;

namespace Template.UI.Controllers
{
    [RequireComponent(typeof(GameHUDView))]
    public sealed class GameHUDController : MonoBehaviour
    {
        private GameHUDView m_View;

        private void Awake()
        {
            m_View = GetComponent<GameHUDView>();
        }

        private void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null || m_View == null) return;

            var currentState = gm.CurrentState?.GetType().Name ?? "None";
            var activeSeat = gm.ActiveTurn.ToString();
            var targetColour = gm.TargetColour.ToString();
            var playerHand = gm.PlayerHand != null ? gm.PlayerHand.Count : 0;
            var aiHand = gm.OpponentHandSize;

            var stateText = $"State: {currentState} | Active: {activeSeat}";
            var infoText = $"Target: {targetColour} | Player: {playerHand} | AI: {aiHand}";
            var controlsText = "LMB: Play / Pass | RMB: Challenge";

            if (gm.CurrentState is ReactState && gm.ActiveTurn == TurnUser.Opponent)
            {
                controlsText = "LMB: Pass | RMB: Challenge AI";
            }
            else if (gm.CurrentState is DecideState && gm.ActiveTurn == TurnUser.Player)
            {
                controlsText = gm.SelectedCardsCount > 0
                    ? $"LMB Cauldron: Play {gm.SelectedCardsCount} Card(s)"
                    : "LMB Card: Select | LMB Cauldron: Play";
            }

            m_View.UpdateHUD(stateText, infoText, controlsText);
        }
    }
}

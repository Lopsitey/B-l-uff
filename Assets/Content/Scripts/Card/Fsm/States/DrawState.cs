using Content.Scripts.Card.Blackboard;
using UnityEngine;

namespace Bluff.Card.Fsm.States
{
    /// <summary>
    /// Draw phase stub. Fill with deal / top-up rules later.
    /// </summary>
    public sealed class DrawState : ICardRoundState
    {
        private readonly CardRoundFsm m_Fsm;
        private readonly CardRoundBlackboard m_Board;

        public DrawState(CardRoundFsm fsm, CardRoundBlackboard board)
        {
            m_Fsm = fsm;
            m_Board = board;
        }

        public void Enter()
        {
            Debug.Log($"[CardRound] Draw.Enter enemies={m_Board.EnemyCount}");
        }

        public void Tick()
        {
            // TODO: deal cards, then m_Fsm.SetState(new DecideState(...));
        }

        public void Exit()
        {
        }
    }
}

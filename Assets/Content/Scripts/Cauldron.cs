using Template.Content.Scripts.Card.Data;
using Template.Content.Scripts.Managers;
using UnityEngine;

namespace Template
{
    public class Cauldron : MonoBehaviour
    {

        private void OnMouseDown()
        {
            GameManager.Instance.ConfirmPlayerDecision();

        }
    }
}

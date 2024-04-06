using YokaiNoMori.Interface;
using YokaiNoMori.Enumeration;
using UnityEngine;


namespace YokaiNoMori.General
{
    public class FightManager
    {
        public void DoFight(IPawn attack, IPawn defense)
        {
            if (defense.GetPawnType() == EPawnType.Koropokkuru)
            {
                Debug.Log("KORO DEFEATED");
                GameManager.Instance.KorropokuruIsDefeated();
            }

            //Debug.Log($"Attaquant : {attack.GetCurrentOwner().GetName()} avec {attack.GetPawnType()} - Défenseur : {defense.GetCurrentOwner().GetName()} perdant {defense.GetPawnType()}");

            GameManager.Instance.GraveyardManager.SendToGraveyard(defense, attack.GetCurrentOwner());
        }
    }
}

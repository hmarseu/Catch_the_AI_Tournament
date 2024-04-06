using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YokaiNoMori.Enumeration;
using YokaiNoMori.Interface;


namespace YokaiNoMori.General
{
    public class TournamentManager : MonoBehaviour
    {
        public List<CompetitorData> CompetitorsList
        {
            get { return m_competitorsList; }
            private set { m_competitorsList = value; }
        }


        public float TournamentTime
        {
            get { return m_tournamentTime; }
            private set { m_tournamentTime = value; }
        }

        private void Awake()
        {
            CompetitorsList = new List<CompetitorData>();

            GameObject[] items = Resources.LoadAll<GameObject>("Prefabs/IA");
            foreach (var competitor in items)
            {
                CompetitorsList.Add(new CompetitorData() { CurrentCompetitor = competitor.GetComponent<ICompetitor>()});
            }
        }


        private void Update()
        {
            if(!m_tournamentResult)
                TournamentTime += Time.deltaTime;
        }


        public void AddVictory(ECampType camp)
        {
            Debug.Log($"Ajout victoire : {CompetitorsList.First(x => x.CurrentCompetitor == (camp == ECampType.PLAYER_ONE ? GameManager.Instance.CompetitorsManager.PlayerOne : GameManager.Instance.CompetitorsManager.PlayerTwo)).CurrentCompetitor.GetName()}");
            Debug.Log($"Ajout défaite : {CompetitorsList.First(x => x.CurrentCompetitor == (camp == ECampType.PLAYER_ONE ? GameManager.Instance.CompetitorsManager.PlayerTwo : GameManager.Instance.CompetitorsManager.PlayerOne)).CurrentCompetitor.GetName()}");


            CompetitorsList.First(x=>x.CurrentCompetitor == (camp == ECampType.PLAYER_ONE ? GameManager.Instance.CompetitorsManager.PlayerOne : GameManager.Instance.CompetitorsManager.PlayerTwo)).CurrentVictory++;
            CompetitorsList.First(x=>x.CurrentCompetitor == (camp == ECampType.PLAYER_ONE ? GameManager.Instance.CompetitorsManager.PlayerTwo : GameManager.Instance.CompetitorsManager.PlayerOne)).CurrentLose++;
        }

        public void AddError(ICompetitor competitor, EValidationType errorOccured)
        {
            Debug.Log($"Ajout ERREUR {errorOccured} : {competitor.GetName()}");
            CompetitorsList.First(x => x.CurrentCompetitor == competitor).CurrentErrorOccured++;
        }


        public void AddIllegalError(ICompetitor competitor, EValidationType errorOccured)
        {
            AddError(competitor, errorOccured);
            AddVictory(competitor.GetCamp() == ECampType.PLAYER_ONE ? ECampType.PLAYER_TWO : ECampType.PLAYER_ONE);
        }



        public void AddDraw()
        {
            CompetitorsList.First(x => x.CurrentCompetitor == GameManager.Instance.CompetitorsManager.PlayerOne).CurrentDraw++;
            CompetitorsList.First(x => x.CurrentCompetitor == GameManager.Instance.CompetitorsManager.PlayerTwo).CurrentDraw++;
        }

        public void ShowResult()
        {
            m_tournamentResult = true;

            CompetitorsList.OrderBy(x => x.GetScore());

            foreach(CompetitorData competitor in CompetitorsList)
            {
                Debug.Log($"{competitor.CurrentCompetitor.GetName()} : {competitor.GetScore()}. {competitor.CurrentVictory} victory " +
                    $"- {competitor.CurrentLose} loose - {competitor.CurrentDraw} draw - {competitor.CurrentErrorOccured} error occured");
            }
        }

        [SerializeField]
        private List<CompetitorData> m_competitorsList;

        private float m_tournamentTime;
        private bool m_tournamentResult;
    }
}

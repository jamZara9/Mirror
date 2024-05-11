using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using ThunderWire.Attributes;

namespace UHFPS.Runtime
{
    [InspectorHeader("Saves UI Loader")]
    public class SavesUILoader : MonoBehaviour
    {
        public BackgroundFader BackgroundFader;
        public Button ContinueButton;
        public Button LoadButton;

        [Header("Save Slot")]
        public Transform SaveSlotsParent;
        public GameObject SaveSlotPrefab;

        [Header("Settings")]
        public bool FadeOutAtStart;
        public bool LoadAtStart;
        public float FadeSpeed;

        [Header("Events")]
        public UnityEvent OnSavesBeingLoaded;
        public UnityEvent OnSavesLoaded;
        public UnityEvent OnSavesEmpty;

        private readonly Dictionary<GameObject, SavedGameInfo> saveSlots = new();
        private SavedGameInfo? lastSave;
        private SavedGameInfo? selected;

        private bool isLoading;

        private async void Start()
        {
            if (LoadAtStart)
            {
                // load saves process
                await LoadAllSaves();

                // enable or disable continue button when last save exists
                if(ContinueButton != null)
                    ContinueButton.gameObject.SetActive(lastSave.HasValue);

                if (FadeOutAtStart) 
                    StartCoroutine(BackgroundFader.StartBackgroundFade(true));
            }
        }

        public async void LoadSavedGames()
        {
            foreach (var slot in saveSlots)
            {
                Destroy(slot.Key);
            }

            saveSlots.Clear();
            OnSavesBeingLoaded?.Invoke();

            await LoadAllSaves();
        }

        public void LoadLastSave()
        {
            if (!lastSave.HasValue || isLoading) 
                return;

            SaveGameManager.SetLoadGameState(lastSave.Value.Scene, lastSave.Value.Foldername);
            StartCoroutine(FadeAndLoadGame());

            selected = null;
            isLoading = true;
        }

        public void LoadSelectedSave()
        {
            if (!selected.HasValue || isLoading)
                return;

            SaveGameManager.SetLoadGameState(selected.Value.Scene, selected.Value.Foldername);
            StartCoroutine(FadeAndLoadGame());

            selected = null;
            isLoading = true;
        }

        public void ResetSaves()
        {
            if (LoadButton != null) LoadButton.gameObject.SetActive(false);
            foreach (var slot in saveSlots)
            {
                UIButton slotButton = slot.Key.GetComponent<UIButton>();
                slotButton.DeselectButton();
            }

            selected = null;
        }

        private async Task LoadAllSaves()
        {
            // load saves in another thread
            var savedGames = await SaveGameManager.SaveGameReader.ReadAllSaves();

            // set last saved game
            if(savedGames.Length > 0)
                lastSave = savedGames[0];

            // instantiate saves in main thread
            for (int i = 0; i < savedGames.Length; i++)
            {
                SavedGameInfo saveInfo = savedGames[i];
                GameObject slotGO = Instantiate(SaveSlotPrefab, SaveSlotsParent);
                slotGO.name = "Slot" + i.ToString();

                LoadGameSlot loadGameSlot = slotGO.GetComponent<LoadGameSlot>();
                loadGameSlot.Initialize(i, saveInfo);

                UIButton loadButton = slotGO.GetComponent<UIButton>();
                loadButton.OnClick.AddListener((_) => 
                {
                    selected = saveInfo;
                    if (LoadButton != null) LoadButton.gameObject.SetActive(true);
                });

                saveSlots.Add(slotGO, saveInfo);
            }

            if(savedGames.Length > 0)
            {
                OnSavesLoaded?.Invoke();
            }
            else
            {
                OnSavesEmpty?.Invoke();
            }
        }

        IEnumerator FadeAndLoadGame()
        {
            if(BackgroundFader != null) yield return BackgroundFader.StartBackgroundFade(false, fadeSpeed: FadeSpeed);
            SceneManager.LoadScene(SaveGameManager.LMS);
        }
    }
}
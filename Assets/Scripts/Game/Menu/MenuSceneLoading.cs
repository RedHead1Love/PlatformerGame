using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Foxberry
{
    internal sealed class MenuSceneLoading
    {
        private readonly GameObject _loadingPanel;

        public MenuSceneLoading(GameObject loadingPanel)
        {
            _loadingPanel = loadingPanel;
        }

        public IEnumerator Load(string sceneName)
        {
            if (_loadingPanel != null)
            {
                _loadingPanel.SetActive(true);
            }

            yield return null;
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            if (operation == null)
            {
                yield break;
            }

            while (!operation.isDone)
                yield return null;
        }
    }
}

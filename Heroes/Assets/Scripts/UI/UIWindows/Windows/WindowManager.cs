using System.Collections.Generic;
using System.Linq;
using CoreUtils.Utils;
using DG.Tweening;
using UnityEngine;

namespace UI.Windows
{
    public class WindowManager : MonoSingleton<WindowManager>
    {
         protected override void Init()
        {
            base.Init();
            _ui = UiManager.Instance;
            DontDestroyOnLoad(gameObject);
        }

        private UiManager _ui;
        private readonly List<Window> _stack = new List<Window>();

        public Window LastOpened => _stack.LastOrDefault();

        public Window Open(Window window, bool forced = false, bool animate = true)
        {
            if (!forced && _stack.Count > 0) return null;
            
            if (window.UniqueWindow)
            {
                var availableWindow = _stack.FirstOrDefault(x => x.UniqueWindowPrefab == window);
                if (availableWindow != null)
                    return availableWindow;
            }

            var win = Instantiate(window, _ui.MainCanvas.transform);
            if (win.UniqueWindow)
                win.UniqueWindowPrefab = window;

            if (animate)
            {
                win.transform.localScale = Vector3.zero;
                win.CreateBackground();
                win.transform.DOScale(1.1f, 0.2f).OnComplete(() =>
                {
                    win.transform.DOScale(1f, 0.1f).OnComplete(
                        () => { win.OnOpen(); });
                });
            }

            if (!forced)
                _stack.Add(win);
                
            return win;

        }

        public void Close(Window window)
        {
            if (_stack.Contains(window))
            {
                _stack.Remove(window);
                if (_stack.Count > 0)
                {
                    Open(_stack[0]);
                }
            }

            window.OnClose();
            Destroy(window.gameObject);
        }

        public void CloseAll()
        {
            foreach (var window in _stack)
            {
                if (window == null)
                    continue;
                
                window.OnClose();
                Destroy(window.gameObject);
            }

            _stack.Clear();
        }
    }
}
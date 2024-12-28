mergeInto(LibraryManager.library, {
  ActivateMainMenu: function () {
    window.dispatchReactUnityEvent("ActivateMainMenu");
  },
  ActivatePauseMenu: function (sfxMute, musicMute) {
    window.dispatchReactUnityEvent("ActivatePauseMenu", sfxMute, musicMute);
  },
  ActivateDeathMenu: function () {
    window.dispatchReactUnityEvent("ActivateDeathMenu");
  },
  SubmitTime: function (time) {
    window.dispatchReactUnityEvent("SubmitTime", time);
  },
  TakeScreenshot: function () {
    window.dispatchReactUnityEvent("TakeScreenshot");
  },
});
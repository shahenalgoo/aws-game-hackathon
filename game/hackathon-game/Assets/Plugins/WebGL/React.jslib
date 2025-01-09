mergeInto(LibraryManager.library, {
  ActivateMainMenu: function () {
    window.dispatchReactUnityEvent("ActivateMainMenu");
  },
  ActivatePauseMenu: function (sfxMute, musicMute) {
    window.dispatchReactUnityEvent("ActivatePauseMenu", sfxMute, musicMute);
  },
  DeactivatePauseMenu: function () {
    window.dispatchReactUnityEvent("DeactivatePauseMenu");
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
  PlayVoiceline: function (type) {
    window.dispatchReactUnityEvent("PlayVoiceline", type);
  },
});
mergeInto(LibraryManager.library, {
  ActivateMainMenu: function () {
    window.dispatchReactUnityEvent("ActivateMainMenu");
  },
  SubmitTime: function (time) {
    window.dispatchReactUnityEvent("SubmitTime", time);
  },
  TakeScreenshot: function () {
    window.dispatchReactUnityEvent("TakeScreenshot");
  },
});
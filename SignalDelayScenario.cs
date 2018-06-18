﻿using System.IO;
using UnityEngine;
using KSP.UI.Screens;
using CommNet;

namespace SignalDelay
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.FLIGHT)]
    public class SignalDelayScenario : ScenarioModule
    {
        #region LIFE CYCLE METHODS

        IButton toolbarButton;
        ApplicationLauncherButton appLauncherButton;

        public void Start()
        {
            GameEvents.onVesselSwitching.Add(OnVesselSwitching);

            // Setup Blizzy's Toolbar button
            if (ToolbarManager.ToolbarAvailable)
            {
                Core.Log("Registering Blizzy's Toolbar button...", Core.LogLevel.Important);
                toolbarButton = ToolbarManager.Instance.add("SignalDelay", "SignalDelay");
                toolbarButton.Text = "Signal Delay";
                toolbarButton.TexturePath = "SignalDelay/icon24";
                toolbarButton.ToolTip = "Switch Signal Delay";
                toolbarButton.OnClick += (e) => { ToggleMod(); };
            }

            // Setup AppLauncher button
            if (SignalDelaySettings.AppLauncherButton)
            {
                Core.Log("Registering AppLauncher button...", Core.LogLevel.Important);
                Texture2D icon = new Texture2D(38, 38);
                icon.LoadImage(File.ReadAllBytes(System.IO.Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "icon128.png")));
                appLauncherButton = ApplicationLauncher.Instance.AddModApplication(ToggleMod, ToggleMod, null, null, null, null, ApplicationLauncher.AppScenes.FLIGHT, icon);
            }
        }

        public void OnDisable()
        {
            GameEvents.onVesselSwitching.Remove(OnVesselSwitching);
            if (toolbarButton != null) toolbarButton.Destroy();
            if ((appLauncherButton != null) && (ApplicationLauncher.Instance != null))
                ApplicationLauncher.Instance.RemoveModApplication(appLauncherButton);
            Active = false;
        }

        /// <summary>
        /// Deactivates signal delay before switching vessels
        /// </summary>
        public void OnVesselSwitching(Vessel from, Vessel to) => Active = false;

        /// <summary>
        /// Checks key presses and SAS mode changes and queues them for execution if signal delay is active
        /// </summary>
        public void Update()
        {
            //if (GameSettings.LANDING_GEAR.GetKeyDown()) SignalDelaySettings.IsEnabled = !SignalDelaySettings.IsEnabled;  // -- COMMENT AFTER TEST!!!
            if (!Active) return;

            // Checking if kOS terminal is focused and locks control => ignoring input then
            if (InputLockManager.lockStack.ContainsKey("kOSTerminal")) return;

            // Checking all key presses and enqueing corresponding actions
            if (GameSettings.LAUNCH_STAGES.GetKeyDown()) Enqueue(CommandType.LAUNCH_STAGES);
            if (GameSettings.PITCH_DOWN.GetKey()) Enqueue(CommandType.PITCH_DOWN);
            if (GameSettings.PITCH_UP.GetKey()) Enqueue(CommandType.PITCH_UP);
            if (GameSettings.YAW_LEFT.GetKey()) Enqueue(CommandType.YAW_LEFT);
            if (GameSettings.YAW_RIGHT.GetKey()) Enqueue(CommandType.YAW_RIGHT);
            if (GameSettings.ROLL_LEFT.GetKey()) Enqueue(CommandType.ROLL_LEFT);
            if (GameSettings.ROLL_RIGHT.GetKey()) Enqueue(CommandType.ROLL_RIGHT);
            if (GameSettings.TRANSLATE_FWD.GetKey()) Enqueue(CommandType.TRANSLATE_FWD);
            if (GameSettings.TRANSLATE_BACK.GetKey()) Enqueue(CommandType.TRANSLATE_BACK);
            if (GameSettings.TRANSLATE_DOWN.GetKey()) Enqueue(CommandType.TRANSLATE_DOWN);
            if (GameSettings.TRANSLATE_UP.GetKey()) Enqueue(CommandType.TRANSLATE_UP);
            if (GameSettings.TRANSLATE_LEFT.GetKey()) Enqueue(CommandType.TRANSLATE_LEFT);
            if (GameSettings.TRANSLATE_RIGHT.GetKey()) Enqueue(CommandType.TRANSLATE_RIGHT);
            if (GameSettings.THROTTLE_CUTOFF.GetKeyDown()) Enqueue(CommandType.THROTTLE_CUTOFF);
            if (GameSettings.THROTTLE_FULL.GetKeyDown()) Enqueue(CommandType.THROTTLE_FULL);
            if (GameSettings.THROTTLE_DOWN.GetKey()) Enqueue(CommandType.THROTTLE_DOWN);
            if (GameSettings.THROTTLE_UP.GetKey()) Enqueue(CommandType.THROTTLE_UP);
            if (GameSettings.WHEEL_STEER_LEFT.GetKey()) Enqueue(CommandType.WHEEL_STEER_LEFT);
            if (GameSettings.WHEEL_STEER_RIGHT.GetKey()) Enqueue(CommandType.WHEEL_STEER_RIGHT);
            if (GameSettings.WHEEL_THROTTLE_DOWN.GetKey()) Enqueue(CommandType.WHEEL_THROTTLE_DOWN);
            if (GameSettings.WHEEL_THROTTLE_UP.GetKey()) Enqueue(CommandType.WHEEL_THROTTLE_UP);
            if (GameSettings.HEADLIGHT_TOGGLE.GetKeyDown()) Enqueue(CommandType.LIGHT_TOGGLE);
            if (GameSettings.LANDING_GEAR.GetKeyDown()) Enqueue(CommandType.LANDING_GEAR);
            if (GameSettings.BRAKES.GetKeyDown()) Enqueue(CommandType.BRAKES);
            if (GameSettings.BRAKES.GetKeyUp()) Enqueue(CommandType.BRAKES);
            if (GameSettings.RCS_TOGGLE.GetKeyDown()) Enqueue(CommandType.RCS_TOGGLE);
            if (GameSettings.SAS_TOGGLE.GetKeyDown()) Enqueue(CommandType.SAS_TOGGLE);
            if (GameSettings.SAS_HOLD.GetKeyDown()) Enqueue(CommandType.SAS_TOGGLE);
            if (GameSettings.SAS_HOLD.GetKeyUp()) Enqueue(CommandType.SAS_TOGGLE);
            if (GameSettings.AbortActionGroup.GetKeyDown()) Enqueue(CommandType.ABORT);
            if (GameSettings.CustomActionGroup1.GetKeyDown()) Enqueue(CommandType.ACTIONGROUP1);
            if (GameSettings.CustomActionGroup2.GetKeyDown()) Enqueue(CommandType.ACTIONGROUP2);
            if (GameSettings.CustomActionGroup3.GetKeyDown()) Enqueue(CommandType.ACTIONGROUP3);
            if (GameSettings.CustomActionGroup4.GetKeyDown()) Enqueue(CommandType.ACTIONGROUP4);
            if (GameSettings.CustomActionGroup5.GetKeyDown()) Enqueue(CommandType.ACTIONGROUP5);
            if (GameSettings.CustomActionGroup6.GetKeyDown()) Enqueue(CommandType.ACTIONGROUP6);
            if (GameSettings.CustomActionGroup7.GetKeyDown()) Enqueue(CommandType.ACTIONGROUP7);
            if (GameSettings.CustomActionGroup8.GetKeyDown()) Enqueue(CommandType.ACTIONGROUP8);
            if (GameSettings.CustomActionGroup9.GetKeyDown()) Enqueue(CommandType.ACTIONGROUP9);
            if (GameSettings.CustomActionGroup10.GetKeyDown()) Enqueue(CommandType.ACTIONGROUP10);

            // If the user has changed SAS mode, enqueue this command and reset mode
            if (Vessel.Autopilot.Mode != sasMode)
            {
                Enqueue(CommandType.SAS_CHANGE_MODE, Vessel.Autopilot.Mode);
                Vessel.Autopilot.SetMode(sasMode);
            }
        }

        ScreenMessage delayMsg = new ScreenMessage("", 1, ScreenMessageStyle.UPPER_LEFT);

        void FadeOut(ref float v, float amount)
        {
            if (v > amount) v -= amount;
            else if (v < -amount) v += amount;
            else v = 0;
        }

        public void FixedUpdate()
        {
            CheckVessel();
            if (appLauncherButton != null) appLauncherButton.enabled = IsConnected && IsProbe;
            if (toolbarButton != null) toolbarButton.Enabled = IsConnected && IsProbe;
            Core.Log(Core.FCSToString(Vessel.ctrlState, "Vessel FCS"));
            if (!Active) return;
            delayRecalculated = false;

            FlightCtrlState.pitch = FlightCtrlState.yaw = FlightCtrlState.roll = 0;
            FadeOut(ref FlightCtrlState.wheelSteer, 0.1f);
            FadeOut(ref FlightCtrlState.wheelThrottle, 0.1f);

            while (Planetarium.GetUniversalTime() >= Queue.NextCommandTime) Queue.Dequeue();

            sasMode = Vessel.Autopilot.Mode;

            if (SignalDelaySettings.ShowDelay)
            {
                delayMsg.message = "Delay: " + Core.FormatTime(Delay);
                ScreenMessages.PostScreenMessage(delayMsg);
            }
        }

        #endregion
        #region MOD CONTROL METHODS

        public void ToggleMod() => SignalDelaySettings.IsEnabled = !SignalDelaySettings.IsEnabled;

        bool active;

        /// <summary>
        /// Whether signal delay should be applied
        /// </summary>
        bool Active
        {
            get => active;
            set
            {
                if (value == active) return;
                Core.Log("Active = " + value);
                active = value;
                if (active)
                {
                    Vessel.OnFlyByWire += OnFlyByWire;
                    FlightCtrlState = new FlightCtrlState() { mainThrottle = throttleCache = Vessel.ctrlState.mainThrottle };
                    Core.Log("Cached throttle = " + throttleCache);
                    sasMode = Vessel.Autopilot.Mode;
                    InputLockManager.SetControlLock(SignalDelaySettings.HidePartActions ? ControlTypes.ALL_SHIP_CONTROLS : ControlTypes.ALL_SHIP_CONTROLS ^ (ControlTypes.ACTIONS_SHIP | ControlTypes.TWEAKABLES), "this");
                    if (SignalDelaySettings.DebugMode) Core.ShowNotification("Signal delay activated.");
                }
                else
                {
                    Vessel.OnFlyByWire -= OnFlyByWire;
                    InputLockManager.RemoveControlLock("this");
                    if (SignalDelaySettings.DebugMode) Core.ShowNotification("Signal delay deactivated.");
                }
            }
        }

        public bool IsConnected => Vessel?.Connection?.IsConnected ?? false;
        public bool IsProbe => (Vessel.Connection.ControlState & VesselControlState.Probe) == VesselControlState.Probe;

        /// <summary>
        /// Checks whether signal delay should be applied to the active vessel
        /// </summary>
        public void CheckVessel() => Active = SignalDelaySettings.IsEnabled && IsConnected && IsProbe;

        #endregion
        #region COMMAND QUEUE METHODS

        /// <summary>
        /// Active vessel's command queue
        /// </summary>
        public CommandQueue Queue
        {
            get
            {
                foreach (VesselModule vm in Vessel.vesselModules)
                    if (vm is SignalDelayVesselModule) return ((SignalDelayVesselModule)vm).Queue;
                return null;
            }
            set
            {
                foreach (VesselModule vm in Vessel.vesselModules)
                    if (vm is SignalDelayVesselModule)
                    {
                        ((SignalDelayVesselModule)vm).Queue = value;
                        return;
                    }
            }
        }

        /// <summary>
        /// Adds a command with possible parameters to the queue
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="par"></param>
        void Enqueue(CommandType commandType, params object[] par)
        {
            double time = Planetarium.GetUniversalTime();
            Core.Log("Adding command " + commandType + " at " + time + ".");
            Command c = new Command(commandType, time + Delay);
            foreach (object p in par) c.Params.Add(p);
            Queue.Enqueue(c);
        }

        #endregion
        #region VESSEL METHODS

        Vessel Vessel => FlightGlobals.ActiveVessel;

        bool delayRecalculated = false;
        double delay;

        /// <summary>
        /// Current signal delay in seconds
        /// </summary>
        public double Delay
        {
            get
            {
                if (!delayRecalculated) CalculateDelay();
                return delay;
            }
            set
            {
                delay = value;
                delayRecalculated = true;
            }
        }

        void CalculateDelay()
        {
            if (Vessel?.Connection?.ControlPath == null)
            {
                Core.Log("Cannot access control path for " + Vessel?.vesselName + ", delay set to 0.", Core.LogLevel.Error);
                Delay = 0;
                return;
            }
            double dist = 0;
            foreach (CommLink l in Vessel.Connection.ControlPath)
                dist += Vector3d.Distance(l.a.position, l.b.position);
            Delay = dist / Core.LightSpeed;
        }

        public static FlightCtrlState FlightCtrlState { get; set; } = new FlightCtrlState();
        float throttleCache;
        VesselAutopilot.AutopilotMode sasMode;
        bool sasPaused = false;

        /// <summary>
        /// Updates FlightCtrlState for the active vessel
        /// </summary>
        /// <param name="fcs"></param>
        public void OnFlyByWire(FlightCtrlState fcs)
        {
            if (Active)
            {
                if (Core.IsLogging()) Core.Log(Core.FCSToString(FlightCtrlState, "SignalDelay FCS"));
                if (Vessel.Autopilot.Enabled && sasMode == VesselAutopilot.AutopilotMode.StabilityAssist && (FlightCtrlState.pitch != 0 || FlightCtrlState.yaw != 0 || FlightCtrlState.roll != 0))
                {
                    Core.Log("User is steering the vessel in StabilityAssist mode. Temporarily disabling autopilot.");
                    Vessel.Autopilot.Disable();
                    sasPaused = true;
                }
                else if (sasPaused)
                {
                    Core.Log("No user steering. Re-enabling autopilot.");
                    Vessel.Autopilot.Enable();
                    sasPaused = false;
                }

                fcs.pitch += FlightCtrlState.pitch;
                fcs.yaw += FlightCtrlState.yaw;
                fcs.roll += FlightCtrlState.roll;
                if (fcs.mainThrottle == throttleCache) fcs.mainThrottle = FlightCtrlState.mainThrottle;
                else
                {
                    Core.Log("Throttle has been changed from " + throttleCache + " to " + fcs.mainThrottle + " by another mod.");
                    FlightCtrlState.mainThrottle = throttleCache = fcs.mainThrottle;
                }
                fcs.wheelSteer = FlightCtrlState.wheelSteer;
                fcs.wheelThrottle = FlightCtrlState.wheelThrottle;
            }
        }
        #endregion
    }
}

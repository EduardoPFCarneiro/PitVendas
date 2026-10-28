import type { ThemeTypes } from "@/types/themeTypes/ThemeType";

const BLUE_THEME: ThemeTypes = {
  name: "BLUE_THEME",
  dark: false,
  variables: {
    "border-color": "#e5eaef",
    "border-opacity": 1,
  },
  colors: {
    primary: "#0d86d9",
    secondary: "#4ce0a5",
    info: "#46caeb",
    success: "#20c997",
    warning: "#f8c076",
    error: "#fb977d",
    indigo: "#8763da",

    lightprimary: "#e5f3fb",
    lightinfo: "#e1f5fa",
    lightsecondary: "#e3fbf1",
    lightsuccess: "#dffff3",
    lighterror: "#ffede9",
    lightwarning: "#fff6ea",
    lightindigo: "#f1ebff",

    textPrimary: "#2A3547",
    textSecondary: "#2A3547",
    borderColor: "#e5eaef",
    inputBorder: "#DFE5EF",
    containerBg: "#ffffff",
    background: "#eef2f6",
    hoverColor: "#f6f9fc",
    surface: "#fff",
    grey100: "#707a82",
    grey200: "#111c2d",
    darkbg: "#2a3447",
    bglight: "#f5f8fb",
    bgdark: "#111c2d",
  },
};

export { BLUE_THEME };

function GetTimezone() {
    return Intl.DateTimeFormat().resolvedOptions().timeZone;
}
options bufsize=32768 pagesize=1000;

data dev.date_format_datetime(drop=v);
    format DATETIME DATETIME.
      DATETIME7 DATETIME7. DATETIME8 DATETIME8. DATETIME9 DATETIME9.
      DATETIME10 DATETIME10. DATETIME11 DATETIME11. DATETIME12 DATETIME12. DATETIME13 DATETIME13. DATETIME14 DATETIME14.
      DATETIME15 DATETIME15. DATETIME16 DATETIME16. DATETIME17 DATETIME17. DATETIME18 DATETIME18. DATETIME19 DATETIME19.
      DATETIME20 DATETIME20. DATETIME21 DATETIME21. DATETIME22 DATETIME22. DATETIME23 DATETIME23. DATETIME24 DATETIME24.
      DATETIME7_1 DATETIME7.1 DATETIME8_1 DATETIME8.1 DATETIME9_1 DATETIME9.1
      DATETIME10_1 DATETIME10.1 DATETIME11_1 DATETIME11.1 DATETIME12_1 DATETIME12.1 DATETIME13_1 DATETIME13.1 DATETIME14_1 DATETIME14.1
      DATETIME15_1 DATETIME15.1 DATETIME16_1 DATETIME16.1 DATETIME17_1 DATETIME17.1 DATETIME18_1 DATETIME18.1 DATETIME19_1 DATETIME19.1
      DATETIME20_1 DATETIME20.1 DATETIME21_1 DATETIME21.1 DATETIME22_1 DATETIME22.1 DATETIME23_1 DATETIME23.1 DATETIME24_1 DATETIME24.1
      DATETIME7_2 DATETIME7.2 DATETIME8_2 DATETIME8.2 DATETIME9_2 DATETIME9.2
      DATETIME10_2 DATETIME10.2 DATETIME11_2 DATETIME11.2 DATETIME12_2 DATETIME12.2 DATETIME13_2 DATETIME13.2 DATETIME14_2 DATETIME14.2
      DATETIME15_2 DATETIME15.2 DATETIME16_2 DATETIME16.2 DATETIME17_2 DATETIME17.2 DATETIME18_2 DATETIME18.2 DATETIME19_2 DATETIME19.2
      DATETIME20_2 DATETIME20.2 DATETIME21_2 DATETIME21.2 DATETIME22_2 DATETIME22.2 DATETIME23_2 DATETIME23.2 DATETIME24_2 DATETIME24.2
      DATETIME7_3 DATETIME7.3 DATETIME8_3 DATETIME8.3 DATETIME9_3 DATETIME9.3
      DATETIME10_3 DATETIME10.3 DATETIME11_3 DATETIME11.3 DATETIME12_3 DATETIME12.3 DATETIME13_3 DATETIME13.3 DATETIME14_3 DATETIME14.3
      DATETIME15_3 DATETIME15.3 DATETIME16_3 DATETIME16.3 DATETIME17_3 DATETIME17.3 DATETIME18_3 DATETIME18.3 DATETIME19_3 DATETIME19.3
      DATETIME20_3 DATETIME20.3 DATETIME21_3 DATETIME21.3 DATETIME22_3 DATETIME22.3 DATETIME23_3 DATETIME23.3 DATETIME24_3 DATETIME24.3
      ;

    input v DATETIME22.3;
    DATETIME=v;
    DATETIME7=v; DATETIME8=v; DATETIME9=v;
    DATETIME10=v; DATETIME11=v; DATETIME12=v; DATETIME13=v; DATETIME14=v;
    DATETIME15=v; DATETIME16=v; DATETIME17=v; DATETIME18=v; DATETIME19=v;
    DATETIME20=v; DATETIME21=v; DATETIME22=v; DATETIME23=v; DATETIME24=v;
    DATETIME7_1=v; DATETIME8_1=v; DATETIME9_1=v;
    DATETIME10_1=v; DATETIME11_1=v; DATETIME12_1=v; DATETIME13_1=v; DATETIME14_1=v;
    DATETIME15_1=v; DATETIME16_1=v; DATETIME17_1=v; DATETIME18_1=v; DATETIME19_1=v;
    DATETIME20_1=v; DATETIME21_1=v; DATETIME22_1=v; DATETIME23_1=v; DATETIME24_1=v;
    DATETIME7_2=v; DATETIME8_2=v; DATETIME9_2=v;
    DATETIME10_2=v; DATETIME11_2=v; DATETIME12_2=v; DATETIME13_2=v; DATETIME14_2=v;
    DATETIME15_2=v; DATETIME16_2=v; DATETIME17_2=v; DATETIME18_2=v; DATETIME19_2=v;
    DATETIME20_2=v; DATETIME21_2=v; DATETIME22_2=v; DATETIME23_2=v; DATETIME24_2=v;
    DATETIME7_3=v; DATETIME8_3=v; DATETIME9_3=v;
    DATETIME10_3=v; DATETIME11_3=v; DATETIME12_3=v; DATETIME13_3=v; DATETIME14_3=v;
    DATETIME15_3=v; DATETIME16_3=v; DATETIME17_3=v; DATETIME18_3=v; DATETIME19_3=v;
    DATETIME20_3=v; DATETIME21_3=v; DATETIME22_3=v; DATETIME23_3=v; DATETIME24_3=v;

    datalines;
17MAR2013:19:53:01.321

01JAN1582:00:00:00.001
31DEC1582:23:59:59.999
31DEC1959:23:59:59.999
01JAN1960:00:00:00.001
31DEC1969:23:59:59.999
01JAN1970:00:00:00.000
01JAN9999:00:00:00.000
31DEC9999:00:00:00.000

30NOV2019:00:00:01.123
25OCT2019:00:00:10.234
22SEP2019:00:01:00.456
19AUG2019:00:10:00.567
15JUL2019:01:00:00.678
13JUN2019:10:00:00.789
10MAY2019:12:34:56.890
09APR2019:12:34:56.987
02MAR2019:12:34:56.654
01FEB2019:12:34:56.321

17MAR2013:00:00:00.000
17MAR2013:00:00:00.001
17MAR2013:00:00:00.05
17MAR2013:00:00:01.02
17MAR2013:00:00:59.50
17MAR2013:00:01:01.3
17MAR2013:00:25:00.45
17MAR2013:00:29:35
17MAR2013:00:31:15
17MAR2013:00:41:40.45
17MAR2013:00:59:59.9321
17MAR2013:00:59:59.9875
17MAR2013:00:59:59.9987
17MAR2013:00:59:59.9999
17MAR2013:01:23:45.6789
17MAR2013:02:05:00.45
17MAR2013:02:21:40.45
17MAR2013:09:29:59
17MAR2013:09:31:01
17MAR2013:09:59:59.9321
17MAR2013:09:59:59.9875
17MAR2013:09:59:59.9987
17MAR2013:09:59:59.9999
17MAR2013:10:00:03.60
17MAR2013:10:08:23.65
17MAR2013:10:41:43.85
17MAR2013:11:29:59
17MAR2013:11:31:01
17MAR2013:11:59:58.954
17MAR2013:11:59:58.965
17MAR2013:11:59:58.953
17MAR2013:11:59:58.964
17MAR2013:11:59:59.9321
17MAR2013:11:59:59.9875
17MAR2013:11:59:59.9987
17MAR2013:11:59:59.9999
17MAR2013:12:00:00.00
17MAR2013:12:00:00.01
17MAR2013:12:00:00.954
17MAR2013:12:00:00.965
17MAR2013:12:00:01.953
17MAR2013:12:00:01.964
17MAR2013:12:21:44.45
17MAR2013:12:21:44.45
17MAR2013:19:18:26.05
17MAR2013:19:18:26.15
17MAR2013:19:18:26.25
17MAR2013:19:18:26.35
17MAR2013:19:18:26.45
17MAR2013:19:18:26.55
17MAR2013:19:18:26.65
17MAR2013:19:18:26.75
17MAR2013:19:18:26.85
17MAR2013:19:18:26.95
17MAR2013:19:18:27.05
17MAR2013:19:18:27.15
17MAR2013:19:18:27.25
17MAR2013:19:18:27.35
17MAR2013:19:18:27.45
17MAR2013:19:18:27.55
17MAR2013:19:18:27.65
17MAR2013:19:18:27.75
17MAR2013:19:18:27.85
17MAR2013:19:18:27.95
17MAR2013:19:54:32.1
17MAR2013:23:29:59
17MAR2013:23:31:01
17MAR2013:23:59:58.954
17MAR2013:23:59:58.965
17MAR2013:23:59:58.953
17MAR2013:23:59:58.964
17MAR2013:23:59:59.9321
17MAR2013:23:59:59.9875
17MAR2013:23:59:59.9987
17MAR2013:23:59:59.9999
;
run;
	
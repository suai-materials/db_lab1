   M S B A M L   `   `   `  ????  DataBase    DataBase.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	     ? Title$  
MainWindow??5   	   $	??450??6   $	??800??6)   .??5      ?  ? RowDefinitions 5	   
   ?? 5
      $	??10* ?6   ?? 5      $??1* ?6   ?? 5      $??1* ?6   ?? 5      $??1* ?6   .+?5   
   &   System.Windows.Controls.DataGrid  -   5        $
??data??5   :   ?? 5   
   $??1??6   $??Center=?6"   $??Stretch=?6=    ?? Orientation$ 
Horizontal=?6[   .+?5      ?? Фамилия:6"   ?? 5      -   5        $??Familia??5      $?? ??6%   $	??300??6-   ?? 5      -   5        .??5   $   
Поиск6,   ?? 5      -   5        $??YoungMen??5      $&?? Юноши с возрастом??5      $??50, 0, 0, 0q?63   5      ?? 5   
   $??2??6   $??Center=?6"   $??Stretch=?6=   $ 
Horizontal=?6[   .+?5      ?? -   5        $??GroupSearch??5      $"??Поиск по группе??5   [   5      ?? 5      -   5        $??	GroupText??5      $y?	Collapsed=?6)   $??20, 0, 0, 0q?6@   $??Номер группы:??6U   ?? 5      -   5        $??Group??5   -   $y?	Collapsed=?6   $	??300??6:   ?? 5      -   5        $??GroupButton??5   ,   $y?	Collapsed=?6   .??6U   
Поиск6]   ?? 5      -	   5        $??	OutOfTown??5   ,   $??10, 0, 0, 0q?6   $-??'Иногородние студенты??5   9   ?? 5      -
   5        $??
Reform1Btn??5   %   $??10, 0, 0, 0q?67   $9??3Образовательная реформа № 1??6L   5       ?? 5!   
   $??3??6   $??Center=?6"   $??Stretch=?6=   $ 
Horizontal=?6[   .+?5"      ?? -   5        $??
YearSearch??5#   8   $/??)Поиск по году рождения??6   68   ?? 5$      -   5        $??YearText??5$      $y?	Collapsed=?6(   $??20, 0, 0, 0q?6?   $??Год рождения:??6T   ?? 5%      -   5        $
??Year??5%   -   $y?	Collapsed=?6   $	??300??69   ?? 5&      -   5        $??
YearButton??5&   G   $y?	Collapsed=?6   .??6Y   
Поиск6a   ?? 5'      -   5        $??
Reform2Btn??5'   %   $??10, 0, 0, 0q?67   $9??3Образовательная реформа № 2??6L   ?? 5(      -   5        $??ExitBtn??5(   "    ?? IsCancel$
 True??61   $??10, 0, 0, 0q?6A   $/??)Войти в другой аккаунт??6V   ?? 5)      -   5        $??ExcelImportBtn??5)   $   $
 True??6:   $??10, 0, 0, 0q?6J   $??Import to Excel??6_   5+      5,      5-      
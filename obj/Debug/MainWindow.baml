   M S B A M L   `   `   `  ����	  DJHCP    DJHCP.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -   5          � Title$  
DJHCP v5.1��5   	   $	��500��6   $	��800��6)    � 
ResizeMode$ CanResizeWithGrip=�6N   .��5      �  � ColumnDefinitions 5	   
   �� 5
      $��1* �6   �� 5      $��2* �6   �� 5      $��1* �6    � RowDefinitions 5   
   �� 5      $��1* �6   �� 5      $��4* �6   �� 5      $��1* �6   .+�5   
   �� -   5         ��Name$ Button��5      $��Add Customs��6!   $��5,5,5,10q�67   $��20(�6[    �� 	IsDefault .  J� 	AllowDrop . $��1��6�   $��2��6�   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ TrackListing��5      $��10,5,5,5q�6)   $ �� IsSynchronizedWithCurrentItem$ FalseY�5      	   AutoGenerateColumns	 .  
   CanUserAddRows
 .     CanUserReorderColumns .    CanUserDeleteRows .  $��2��6�      
IsReadOnly . $��1��6     Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Id��6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 61   ��  �� XPath$ IDTag��  5      $ Folder location��6%    6>   �� $ FolderLocation��5      �� 5   
   -   5        $��Tracklist Generator��5      $��	5,5,10,10q�6/   $��20(�6B   $��2��6s   $��2��6�   �� 5   
   -   5         ��Name$ proprieties��5      $��	5,36,10,5q�6'   $��2��6:   $��1��6J   �� 5   
   -   5        $��Open Extracted files��5      $��	5,25,10,5q�60   $��20(�6C   $��2��6w   �� 5   
   -   5         ��Name$ 	SearchBar��5      $��	63,49,5,5q�6%    �� TextWrapping$
 Wrap=�68    � ToolTip$  ��6L   $��2��6s   �� 5   
   $��Search:��6   $��	10,49,0,5q�6"   $
��Left=�65   $��48��6P   �� 5    
   $!��Additional Song Information��6   $��5,5,10,0q�66   $��Center=�6H   $��2��6l   $��26��6|   $	��Top=�6�   $��1��6�   �� 5!   
   $��Base Folder Path:��6   $��
10,23,79,0q�6,   $��26��6@   $	��Top=�6L   �� 5"   
   -   5         ��Name$ baseFolderLabel��5"      $��
125,23,5,0q�6)   $��2��6=   $��26��6Q   $	��Top=�6]   �� 5#   
   -	   5        $ EditTrackData��5#      $��Edit TRAC.csv data��6(   $��	10,5,5,10q�6E   $��20(�6X   $��2��6�   o� 5$   
   $��3��6   $��20��6#   $	��Top=�6/   .@�5%      m� $
��File��6   .@�5&      m� -
   5        $��Load Extracted Files��5&      69   m� 5'      -   5        $��Add Customs��5'      60   m� 5(      -   5        $��Update��5(      6+   �� 5)      m� 5*      $��Close��6   5+      m� 5,      $��Tools��6   .@�5-      m� -   5        $��Tracklisting generator��5-      6;   m� 5.      -   5        $��Edit TRAC.csv data��5.      67   m� 5/      -   5        $*��$Rename to Uppercase (PS3/RPCS3 only)��5/      6I   50      m� 51      $��About��6   52      53      54      
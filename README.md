# FloorplanningDesigner
## 簡介
　　此專案為專題的延伸，使用C#進行撰寫的一個簡易Floorplanning Tool，一個平面規劃結果分為兩個檔案設定檔以及佈局檔，分別為CAD Contest中的輸入檔案和輸出檔案。設定檔包含晶片大小、soft module最小尺寸、fixed module的位置以及寬高，而佈局檔包含每個soft module的位置，可供使用者自行新增設定檔以及擁有圖形化的介面供使用者進行平面規劃。此外，也提供自動平面規劃的功能。


## 功能介紹
　　在Floorplanning Designer中提中使用者自訂設定檔，可自由設定晶片大小、soft module最小尺寸、fixed module的位置以及寬高。在使用者匯入設定檔後，在畫面的左方可以在晶片上進行對進行module擺放，提供在每個位置上的點選左鍵進行新增和點選右鍵進行刪減，且滑鼠長按後可形成一個虛線所形成的矩形可在選取到範圍的進行新增和刪減，也可滑鼠中鍵長按進行整個module的位移；支援立即性的顯示當前平面規劃結果的錯誤，提供儲存佈局檔、另存佈局檔、儲存佈局圖片和儲存錯誤清單的功能，同時也支援上一步及下一步的功能；提供一鍵平面規劃的功能，利用專題中的演算法進行放置，會根據當前的設定檔進行平面規劃，進而來達到"自動"平面規劃的效果。

## 執行方式
　　編譯程式後，需將編譯後的執行檔與placement.exe放在同一個目錄下才可進行自動平面規劃，依序匯入設定檔(input file)、布局檔(output file)，可自訂設定檔與布局檔，也可從testcase資料夾中直接匯入。

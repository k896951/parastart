# parastart
parastartコマンドは、指定のプログラムを同時にバックグラウンドで実行し、それらが全て終了するまで待たせることができます。

## 使い方

a.bat と b.bat と c.bat  を並列で実行し、全ての実行が終了したら d.bat を実行したい。  

以下の例では、a.bat b.bat c.bat をバックグラウンドで起動し d.bat を起動しています。  
a.bat b.bat c.bat それぞれの実行終了を待たずに d.bat が実行されることになります。

    @echo off
    start a.bat
    start b.bat
    start c.bat
    call d.bat

parastartコマンドを使えば a.bat b.bat c.bat が終了するまで d.bat の実行が行われません。

    @echo off
    parastart a.bat b.bat c.bat
    call d.bat	

## 実行時の制限

parastartコマンドは、ハイフン( - )から始まる引数はオプションとみなします。そのため、先頭にハイフンのある引数を、バッチファイル名やプログラム名として指定できません。

この例では、-a.bat -b.bat -c.bat はオプションとして解釈されます。ですが正しいオプションではないため無視され、-a.bat -b.bat -c.bat は実行されません。

    parastart -wait 180 -a.bat -b.bat -c.bat

どうしてもハイフンから始まる名称を持つバッチファイルやプログラムを実行したい場合は、以下のようにファイル名を書く直前に “dummyfile” を記述するか、リターンコードゼロを返すバッチファイル(この例ではdummy.bat)を記述してください。

    parastart -wait 180 dummyfile -a.bat -b.bat -c.bat
    parastart -wait 180 dummy.bat -a.bat -b.bat -c.bat

前者の記述だと、dummyfile の後ろの -a.bat -b.bat -c.bat が実行されます。
後者の記述だと、dummy.bat -a.bat -b.bat -c.bat が実行されます。

parastartコマンドが引数を解析する際、最初のバッチファイル名/プログラム名/dummyfile を見つけると、以降に続く引数は全てバッチファイル名/プログラム名だとみなして処理します。

dummyfile はハイフンから始まるファイル名を指定するために用意されたダミーファイル名です。同じ名称のファイルがあっても実行しません。引数解析時に読み飛ばしされるだけです。

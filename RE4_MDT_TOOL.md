## RE4_MDT_TOOL

Na pasta RE4_MDT_TOOL você encontra tools destinadas a converter o MDT entre as versões do jogo
<br>e entre as versões MONO e MULTI do arquivo MDT.

**Sobre as versões do MDT:**
<br>Existem duas versões/tipos de MDT:

! **MONO**: o arquivo conta com um único idioma.
<br>! **MULTI**: contém um conjunto de arquivos MONO. No caso, são de 6 ou 8 idiomas dentro desse arquivo.
<br>(Para saber qual é qual, os arquivos multi sempre começam com 06000000 ou 00000006)

No arquivo MULTI a ordem dos idiomas é a seguinte:

0) Japanese
1) English
2) French
3) German
4) Italian
5) Spanish
6) Chinese (ID 6)
7) Chinese (ID 9)

Nota: O idioma "Chinese" só está disponível em algumas versões do jogo.
<br>Nota2: O idioma "Japanese", mesmo presente nos arquivos, não funciona em todas as versões do jogo.
<br>Nota3: A versão de PS2/2007 tem somente 6 idiomas e a versão UHD 8, pois tem dois idiomas "Chinese" a mais. A existência ou não deles não faz diferença no funcionamento do jogo.

**Sobre as versões do jogo:**

**BIG:** as tools terminadas em BIG são destinadas às versões big endian do jogo, que são: GC/WII/XBOX360/PS3;
<br>**UHD:** são as tools que contêm o nome UHD. Servem para as versões UHD/PS2/2007. Nota: aqui existem duas tools do tipo MULTI: uma com 6 e outra com 8 idiomas. A de 6 idiomas é para o 2007/PS2 e a de 8 idiomas é para o UHD (esses dois idiomas a mais são os idiomas "Chinese").
<br>**PS4:** são destinadas para as versões de PS4 (CUSA04704 e CUSA04885)
<br>**NS:** são destinadas para as versões de Nintendo Switch, e a versão de PS4 CUSA09844
<br>**PS4NS:** serve para as duas versões citadas acima.

**Explicação de cada Tool:**
<br>Nota: Omitirei o nome da versão do jogo, mas você deve usar a tool que tem o mesmo nome que a versão que você está usando.

**=> RE4_MDT_SPLIT_**
<br>Essa tool divide o arquivo MDT MULTI em arquivos MDT MONO.
<br>Para usar, arreste e solte sobre a tool um arquivo MDT MULTI. Por exemplo:
<br>para o arquivo "example.MDT" vão ser gerados os arquivos:
<br>* example.0_Japanese.mdt
<br>* example.1_English.mdt
<br>* example.2_French.mdt
<br>* example.3_German.mdt
<br>* example.4_Italian.mdt
<br>* example.5_Spanish.mdt
<br>* example.6_Chinese_zh_tw.mdt
<br>* example.9_Chinese_zh_cn.mdt
<br>Cada um desses arquivos é do tipo MDT MONO
<br>Nota: você não pode renomear os nomes dos arquivos, porque senão não serão reconhecidos na hora de juntá-los novamente.
<br>Nota: os idiomas que você não for usar, pode deletar pois dentro do MDT MULTI esse idioma vai ficar com zero entries;

Para juntar novamente os arquivos, use a tool:
<br>**=> RE4_MDT_MERGE_MULTI_**
<br>Para usar, arreste e solte sobre a tool um dos arquivos MDT MONO
<br>que o programa vai reconhecer os outros pelo nome dos arquivos.

**=> RE4_MDT_SINGLE_MULTI_**
<br>esse faz o mesmo que o de cima, porém o arquivo MDT MONO que você usará, será colocado para ser usado em todos os idiomas.
<br>Exceto nos idiomas japonês e chinês. Esses vão ficar com zero entries.

**=> RE4_MDT_CHOICE_**
<br> Essa tool é a versão evoluída da tool RE4_MDT_SINGLE_MULTI que ao extrair, vai gerar um arquivo idxchoicemdt (que será usado para o repack).
<br> Esse arquivo é usado para definir quais arquivos MDT MONO serão usados em cada idioma.
<br> Nota: você pode repetir o mesmo arquivo em mais de um idioma, que ele vai ser colocado uma única vez no MDT MULTI (economizando assim, espaço).
<br> Nota2 : você também pode colocar "null" no lugar do nome do arquivo, que aí esse idioma vai ficar sem texto.

**=>RE4_MDT_PARSE_**
<br>Essa tool serve para converter o MDT de uma versão do jogo para outra versão do jogo.

**Tutorial By JADERLINK**
<br>**Tools By JADERLINK**
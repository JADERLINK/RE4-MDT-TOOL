# RE4-MDT-TOOL
Extract and repack RE4 MDT files [Edit the game texts] (RE4 2007/PS2/UHD/PS4/NS/GC/WII/X360)

**Translate from Portuguese Brazil**

Programa destinado a extrair e reempacotar arquivos MDT;
<br> O arquivo MDT é o arquivo que contém os textos do jogo;

## Info:

! RE4_MDT_TOOL -> Contém programas destinados a converter o MDT de uma versão para outra;
<br>! RE4_MDT_EDIT -> Contém programas destinados a extrair o arquivo para texto legível em UTF8;

**Sobre o arquivo MDT:**

Existem duas versões/tipos de MDT:

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
<br>Nota2: O idioma "Japanese" mesmo presente nos arquivos, não funciona em todas as versões do jogo. 

**Versões do jogo:**

Cada versão do jogo tem uma tool de MDT diferente, pois a estrutura do arquivo é diferente entre as versões. Porém, as tools permitem converter o MDT entre as versões.

**Mais informações:**

Para mais informações, veja os arquivos abaixo desse repositório:
<br>[RE4_MDT_TOOL.md](https://github.com/JADERLINK/RE4-MDT-TOOL/blob/main/RE4_MDT_TOOL.md)
<br>[RE4_MDT_EDIT.md](https://github.com/JADERLINK/RE4-MDT-TOOL/blob/main/RE4_MDT_EDIT.md)
<br>[MdtEncoding.md](https://github.com/JADERLINK/RE4-MDT-TOOL/blob/main/MdtEncoding.md)

**At.te: JADERLINK**
<br>2025-01-10
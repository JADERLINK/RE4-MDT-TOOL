## RE4_MDT_EDIT

Na pasta RE4_MDT_EDIT você encontra tools destinadas a converter o MDT para um texto legível em UTF8;
<br>Tanto para as versões MONO e MULTI do arquivo MDT;

Para mais informações sobre os tipos de MDT e as versões do jogo, veja:
<br>[RE4_MDT_TOOL.md](https://github.com/JADERLINK/RE4-MDT-TOOL/blob/main/RE4_MDT_TOOL.md)

Então, para as tools de edição do texto eu fiz 4 versões:
<br> MONO ou MULTI (para o tipo de arquivo MDT);
<br> Com ou sem "SPLIT_ENTRIES" no nome. Esse define como vai ser extraído o arquivo.

Por exemplo, para o arquivo "example.1_English.mdt" (do tipo MONO):

**=> RE4_MDT_EDIT_MONO_**
<br>vai gerar os arquivos:
<br>* example.1_English.idxmdt : usado para fazer o repack.
<br>* example.1_English.txtmdt : arquivo com os textos. Aqui, cada linha é uma entry.

Já com o executável:
<br>**=> RE4_MDT_EDIT_SPLIT_ENTRIES_MONO_**
<br>vai gerar os seguintes itens:
<br>* example.1_English.idxmdt : usado para fazer o repack.
<br>Uma pasta de nome "example.1_English" e dentro dela vai ter vários arquivos txtmdt enumerados, cada um deles é uma entry.
<br>Nota: aqui nos arquivos txtmdt existe a quebra de linha, mas é apenas visual. O que faz a quebra de linha no jogo é o comando de quebra de linha.

E nas duas tools, o caractere de "tab" é ignorado, então pode ser usado para ter espaçamento visual no arquivo sem afetar no jogo.

Agora, para os arquivos MDT do tipo MULTI são essas as tools:
<br>Por exemplo, para o arquivo "example.mdt" (do tipo MULTI):

**=> RE4_MDT_EDIT_MULTI_**
<br>vai gerar os arquivos:
<br>* example.idxmultimdt : usado para fazer o repack.
<br>* example.1_English.idxmdt : caso você queira fazer o rapack como arquivo MONO.
<br>* example.1_English.txtmdt : arquivo com os textos, aqui cada linha é uma entry.
<br>* example.0_Japanese.idxmdt
<br>* example.0_Japanese.txtmdt
<br>* example.2_French.idxmdt
<br>* example.2_French.txtmdt
<br>* example.3_German.idxmdt
<br>* example.3_German.txtmdt
<br>* example.4_Italian.idxmdt
<br>* example.4_Italian.txtmdt
<br>* example.5_Spanish.idxmdt
<br>* example.5_Spanish.txtmdt
<br>* example.6_Chinese_zh_tw.idxmdt
<br>* example.6_Chinese_zh_tw.txtmdt
<br>* example.9_Chinese_zh_cn.idxmdt
<br>* example.9_Chinese_zh_cn.txtmdt

E para a tool:
<br>**=> RE4_MDT_EDIT_SPLIT_ENTRIES_MULTI_**
<br> Faz o mesmo que a tool " RE4_MDT_EDIT_SPLIT_ENTRIES_MONO_" porém para os arquivos MULTI. Então, no lugar dos arquivos idxmdt, vai ter pasta com as entries enumeradas dentro.

**=> RE4_MDT_EDIT_CHOICE_**
<br> Essa tool é a versão evoluída da tool RE4_MDT_EDIT_MULTI que, ao extrair, vai gerar um arquivo idxchoicemdt (que será usado para o repack).
<br> A lógica é a mesma do RE4_MDT_CHOICE, porém, em vez de extrair mdt, vai ser criado idxmdt e txtmdt no lugar.
<br>Nota: essa tool depende do arquivo "ChoiceEncoding.json" para definir quais arquivos "MdtEncoding*.json" vão ser usados.

## Atenção:
Agora, o que vou explicar abaixo vale para todas as tools citadas acima:
<br>Todo o conteúdo usado para codificar e decodificar o texto fica no arquivo "MdtEncoding*.json", que pode ser editado.
<br>Permitindo assim, que qualquer valor vire o caractere que você quiser.
<br>Aqui separei em dois conceitos:
<br>* Os valores de caracteres individuais, que possuem um único char.
<br>* E os valores de comando (cmd). Que são uma sequência de caracteres, que eu defini para começar com & e terminar com ;

E dentro do jogo, todos os "Code" são valores ushort. No arquivo mdt estão na ordem little endian (ou big endian), porém no arquivo txtmdt represento tudo como big endian.
<br>Exemplo: o código/valor 01 00 fica 0x0001.
<br>* Os códigos de 0x00 até 0x15 não são caracteres, são códigos de controle (por exemplo: quebra de linha, mudar cor do texto, etc).
<br>* Os códigos entre 0x16 até 0x7F não são usados.
<br>* A partir de 0x80 são caracteres impressos na tela.

Então:
<br>* Tudo que for um código de caractere, eu represento com um char UTF8;
<br>* E os valores de comandos(CMD) são usados para representar os códigos de controle, e os caracteres nos quais não tenho um char UTF8;

Como tinha dito, os valores de comandos começam com & e terminam com ;. Tudo que tiver dentro deles, não diferencia maiúsculo de minúsculo.
<br>Então, por exemplo: &newline; e &NEWLINE; são o mesmo comando e são ambos válidos.

Todas as frases começam com 0x00 &start; e terminam com 0x01 &end;. Porém no arquivo txtmdt eles são omitidos, então não são obrigatórios.

Para saber o que cada código de controle serve, veja a página do Zatarita:
<br>https://github.com/Zatarita/re4-wiki/blob/main/Final/PC/mdt.md
<br>https://residentevilmodding.boards.net/thread/15827/mdt-file-specification
<br> Ou em português aqui:
<br>https://jaderlink.blogspot.com/2025/01/re4-mdt-info.html

Você também pode inserir os valores dos códigos diretamente no arquivo txtmdt com &0x80; ou &hx80; que vai inserir no arquivo o código 80 00 que é o caractere de espaço.

Para saber mais sobre o arquivo "MdtEncoding.json" veja aqui:
<br>[MdtEncoding.md](https://github.com/JADERLINK/RE4-MDT-TOOL/blob/main/MdtEncoding.md)

**Tutorial By JADERLINK**
<br>**Tools By JADERLINK**
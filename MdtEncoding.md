## Explicação do arquivo "MdtEncoding.json"
Ele respeita a estrutura de um arquivo json, então você tem que saber como json funciona.

A tag principal é a __"Encoding"__. Tudo está dentro dela.
* tag __"Info"__: dentro dessa tag tem informações sobre o arquivo.
* tag __"Config"__: aqui tem duas propriedades que definem os caracteres de início e fim de comandos: __"CmdStartChar"__ e __"CmdEndChar"__.
* tag __"CharsetList"__: aqui é uma listagem de chave/valor que define o código e o valor que vai no txtmdt, o qual pode ser um único char ou um comando. Nota: os códigos, os char e os nomes dos comandos, não podem ser repetidos. Exemplos: "0003": "\&NewLine;" ou "00A7": "A"
* tag __"ColorList"__: opcional, define os comandos das cores. Exemplo: "0001": "\&Grey1;"
* tag __"AkaCharset"__: opcional, é usado no repack do txtmdt, são "aliases". Aqui você pode repetir o código, é usado para criar outros comandos para os códigos. Você também pode atribuir caracteres individuais, desde que eles não se repitam. Exemplos: "☆": "009F" ou "Estrela": "009F"
* tag __"ExtraCharset"__: opcional, usado somente na extração. Aqui você não pode repetir o código, porém você deve usar o mesmo char ou comando usado em "CharsetList". Esse lista é usada somente para situações muito específicas. (Permite que outro código represente o mesmo caractere, exemplo: "0x0184": "∅" e "0x0185": "∅")
* tag __"Replace"__: opcional, substitui um texto pelo outro, usado para idiomas que têm Letras compostas por mais de um caractere (ou para outros usos gerais). Ao extrair, é substituído o texto da esquerda pela direita, seguindo a ordem de cima para baixo. E ao fazer repack é substituído o texto da direita pelo da esquerda, na ordem de baixo para cima. É substituída todas as ocorrências encontradas e faz diferenciação de maiúsculas com minusculas. É recomendado usar somente se necessário, pois após substituído o texto, são feitas as validações, então se algo que deveria ser substituído não for, vai causar erro na validação. Exemplo: "&กี่;" : "กี่"

**=> sobre o arquivo txtmdt**
<br>se no seu editor de texto você definir a sintaxe como html, os comandos vão ficar com uma cor diferente. Isso ajuda na edição.

### Atenção:
As tools do tipo MONO só fazem uso do arquivo "MdtEncoding.json".
<br> Já as tools do tipo MULTI, fazem uso de 4 arquivos:

* MdtEncodingJapanese.json : para o idioma Japanese
* MdtEncodingChinese6.json : para o idioma Chinese de ID 6
* MdtEncodingChinese9.json : para o idioma Chinese de ID 9
* MdtEncodingLatin.json : para os outros idiomas

Nota: o Japanese e Chinese fazem uso de várias tabelas de caracteres, então você deve mudar o json para cada arquivo MDT que você for extrair.
<br>Nota2: use o mesmo arquivo MdtEncoding que você usou ao extrair o texto, para fazer o repack do mesmo. Pois se não, pode acontecer erros e o programa não fará o repack do arquivo.

Para a tool RE4_MDT_EDIT_CHOICE é usado o arquivo "ChoiceEncoding.json" para definir os arquivos de "MdtEncoding" usados.

**Tutorial By JADERLINK**
<br>**Tools By JADERLINK**
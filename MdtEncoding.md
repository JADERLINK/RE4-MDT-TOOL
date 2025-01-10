## Explicação do arquivo "MdtEncoding.json"
Ele respeita a estrutura de um arquivo json, então você tem que saber como json funciona.

A tag principal é a "Encoding". Tudo está dentro dela.
* tag "Info": dentro dessa tag tem informações sobre o arquivo.
* tag "Config": aqui tem duas propriedades que definem os caracteres de início e fim de comandos: "CmdStartChar" e "CmdEndChar".
* tag "CharsetList": aqui é uma listagem de chave/valor que define o código e o valor que vai no txtmdt, o qual pode ser um único char ou um comando. Nota: os códigos, os char e os nomes dos comandos, não podem ser repetidos.
* tag "ColorList": opcional, define os comandos das cores.
* tag "AkaCharset": opcional e é usado no repack do txtmdt, são "aliases". Aqui você pode repetir o código, é usado para criar outros comandos para os códigos. Você também pode atribuir caracteres individuais, desde que eles não se repitam. 
* tag "ExtraCharset": opcional, usado somente na extração. Aqui você não pode repetir o código, porém você deve usar o mesmo char ou comando usado em "CharsetList". Esse lista é usada somente para situações muito específicas.

**=> sobre o arquivo txtmdt**
<br>se no seu editor de texto você definir a sintaxe como html, os comandos vão ficar com uma cor diferente. Isso ajuda na edição.

### Atenção:
As tools do tipo MONO só fazem uso do arquivo "MdtEncoding.json".
<br> Já as tools do tipo MULTI, fazem uso de 4 arquivos:

* MdtEncodingJapanese.json : para o idioma Japanese
* MdtEncodingChinese6.json : para o idioma Chinese de ID 6
* MdtEncodingChinese9.json : para o idioma Chinese de ID 9
* MdtEncoding.json : para os outros idiomas

Nota: o Japanese e Chinese fazem uso de várias tabelas de caracteres, então você deve mudar o json para cada arquivo MDT que você for extrair.
<br>Nota2: use o mesmo arquivo MdtEncoding que você usou ao extrair o texto, para fazer o repack do mesmo. Pois se não, pode acontecer erros e o programa não fará o repack do arquivo.

**Tutorial By JADERLINK**
<br>**Tools By JADERLINK**
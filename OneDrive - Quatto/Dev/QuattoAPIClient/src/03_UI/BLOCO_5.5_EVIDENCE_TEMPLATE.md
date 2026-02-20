# ?? BLOCO 5.5 - EVIDENCE TEMPLATE

**Projeto**: Quatto API Client - Configuration Wizard  
**Bloco**: 5.5 - Exemplos Práticos  
**Data de Coleta**: ________________  
**Testador**: ________________  

---

## ?? EVIDÊNCIAS DOS EXEMPLOS

---

## CENÁRIO 1: PAGINAÇÃO OFFSET - EVIDÊNCIAS

### Evidence 1.1: Configuração

```
Valores Configurados:

Base URL:         https://api.gladium.com
Endpoint:         /v1/orders
Pagination Type:  Offset
Page Size:        500
Start Page:       1
Max Pages:        0 (sem limite)

Status: [ ] Validação OK  [ ] Erro
Observações: _____________________________________
```

### Evidence 1.2: Resultado

```
Esperado:  Validação passa, nenhum aviso
Obtido:    [ ] Igual  [ ] Diferente
           _________________________________

Salvo com sucesso?  [ ] SIM  [ ] NÃO
Package OK?         [ ] SIM  [ ] NÃO
```

---

## CENÁRIO 2: PAGINAÇÃO CURSOR - EVIDÊNCIAS

### Evidence 2.1: Configuração

```
Valores Configurados:

Base URL:         https://api.exemplo.com
Endpoint:         /v2/items
Pagination Type:  Cursor
Page Size:        1000
Start Page:       1
Max Pages:        100

Status: [ ] OK  [ ] Erro
Observações: _____________________________________
```

### Evidence 2.2: Cursor Reconhecimento

```
Type "Cursor" foi reconhecido?
[ ] SIM
[ ] NÃO (como foi interpretado?) ________________

Salvou corretamente?  [ ] SIM  [ ] NÃO
Mensagens de erro?    [ ] Nenhuma  [ ] Sim (qual?)
_________________________________________________________________
```

---

## CENÁRIO 3: INCREMENTAL - EVIDÊNCIAS

### Evidence 3.1: Configuração

```
Valores Configurados:

Base URL:              https://api.prod.com
Endpoint:              /v1/customers
Enable Incremental:    [ ] Checked
Watermark Column:      updated_at
System:                ProductionAPI
Environment:           PRD

Status: [ ] OK  [ ] Erro
Observações: _____________________________________
```

### Evidence 3.2: Incremental Status

```
Incremental está ativado?    [ ] SIM  [ ] NÃO
Watermark salvou?            [ ] SIM  [ ] NÃO
System salvou?               [ ] SIM  [ ] NÃO
Environment salvou?          [ ] SIM  [ ] NÃO

Tudo persistiu?
[ ] SIM (100%)
[ ] Parcialmente (o quê não?)
[ ] NÃO (tudo perdido)

Observações: _____________________________________
```

---

## CENÁRIO 4: RAW STORAGE - EVIDÊNCIAS

### Evidence 4.1: Configuração

```
Valores Configurados:

Storage Mode:      FileSystem
Target:            C:\Data\Raw
Compress JSON:     [ ] Checked
Hash JSON:         [ ] Checked

Status: [ ] OK  [ ] Erro
Observações: _____________________________________
```

### Evidence 4.2: Storage Validation

```
FileSystem mode foi aceito?     [ ] SIM  [ ] NÃO
Target path reconhecido?        [ ] SIM  [ ] NÃO
Compress checkbox está checked? [ ] SIM  [ ] NÃO
Hash checkbox está checked?     [ ] SIM  [ ] NÃO

Validação passou?
[ ] SIM (sem avisos)
[ ] COM AVISOS (qual?) ______________________
[ ] ERRO ___________________________________
```

---

## CENÁRIO 5: RETRIES E RATE LIMIT - EVIDÊNCIAS

### Evidence 5.1: Configuração

```
Valores Configurados:

Max Retries:       5
Backoff Mode:      Exponential
Base Delay (ms):   1000
Rate Limit (rpm):  120
Timeout (seg):     180

Status: [ ] OK  [ ] Erro
Observações: _____________________________________
```

### Evidence 5.2: Retry Configuration

```
Max Retries: 5 aceito?        [ ] SIM  [ ] NÃO
Backoff: Exponential OK?      [ ] SIM  [ ] NÃO
Base Delay: 1000 ms OK?       [ ] SIM  [ ] NÃO
Rate Limit: 120 rpm OK?       [ ] SIM  [ ] NÃO
Timeout: 180 seg OK?          [ ] SIM  [ ] NÃO

Todos os valores foram salvos?
[ ] SIM (100%)
[ ] Parcialmente (quais faltaram?)
[ ] NÃO

Mensagens de aviso?
[ ] Nenhuma
[ ] Sim (qual?) _______________________________
```

---

## ?? EVIDENCE SUMMARY

```
Total de Evidências Coletadas por Cenário:

Cenário 1 (Offset):
?? [ ] 1 evidence (Configuração)
?? [ ] 1 evidence (Resultado)

Cenário 2 (Cursor):
?? [ ] 1 evidence (Configuração)
?? [ ] 1 evidence (Reconhecimento)

Cenário 3 (Incremental):
?? [ ] 1 evidence (Configuração)
?? [ ] 1 evidence (Status)

Cenário 4 (Storage):
?? [ ] 1 evidence (Configuração)
?? [ ] 1 evidence (Validation)

Cenário 5 (Retries):
?? [ ] 1 evidence (Configuração)
?? [ ] 1 evidence (Configuration)

TOTAL: ___ evidências

Qualidade das Evidências:
[ ] Excelente (completas e claras)
[ ] Boa (cobre os pontos principais)
[ ] Aceitável (alguns pontos faltam)
[ ] Inadequada (não é suficiente)
```

---

## ?? NOTAS SOBRE EVIDÊNCIAS

```
_________________________________________________________________
_________________________________________________________________
_________________________________________________________________
_________________________________________________________________
```

---

## ?? COLETA ASSINADA POR

```
Testador: ___________________________    Data: ______________
Revisor: ___________________________    Data: ______________
```

---

**Versão**: 3.0.5-BLOCO-5.5-EVIDENCE  
**Status**: ?? PRONTO PARA USAR


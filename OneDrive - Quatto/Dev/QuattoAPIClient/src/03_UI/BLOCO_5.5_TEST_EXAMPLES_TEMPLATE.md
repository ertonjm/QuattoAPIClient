# ?? BLOCO 5.5 - TEST EXAMPLES TEMPLATE

**Projeto**: Quatto API Client - Configuration Wizard  
**Fase**: 3 (Testes)  
**Bloco**: 5.5 - Exemplos Práticos  
**Data de Teste**: ________________  
**Testador**: ________________  

---

## ?? OBJETIVO

Validar que o wizard funciona corretamente com 5 cenários práticos e realistas de configuração.

---

## ?? TEST OVERVIEW

```
???????????????????????????????????????????????????????????
? Cenário                             ? Status   ? Tempo  ?
???????????????????????????????????????????????????????????
? 1. Paginação Offset                 ? ? _____ ? ___ min?
? 2. Paginação Cursor                 ? ? _____ ? ___ min?
? 3. Incremental com Watermark        ? ? _____ ? ___ min?
? 4. Raw Storage                      ? ? _____ ? ___ min?
? 5. Retries e Rate Limit             ? ? _____ ? ___ min?
???????????????????????????????????????????????????????????
? TOTAL                               ? ? _____ ? ___ min?
???????????????????????????????????????????????????????????

Status Legenda: ? PASSOU | ? FALHOU | ?? AVISO
```

---

## CENÁRIO 1: PAGINAÇÃO OFFSET

### Objetivo
Testar configuração de paginação usando offset (página número).

### Configuração

```
Base URL:              https://api.gladium.com
Endpoint:              /v1/orders
Pagination Type:       Offset
Page Size:             500
Start Page:            1
Max Pages:             0 (sem limite)

Expected Result:
?? Validação passa
?? Valores sensatos
?? Pronto para usar
```

### Procedimento

```
1. [ ] Abrir wizard
2. [ ] Aba General:
   [ ] Base URL: https://api.gladium.com
   [ ] Endpoint: /v1/orders
   [ ] Page Size: 500
3. [ ] Aba Paginação:
   [ ] Type: Offset
   [ ] Start Page: 1
   [ ] Max Pages: 0
4. [ ] Clicar OK
5. [ ] Package salva sem erro
```

### Resultado Esperado

```
[ ] Validação aceita valores
[ ] Nenhuma mensagem de aviso
[ ] Wizard fecha normalmente
[ ] Package salva com sucesso
[ ] Sem exceptions na console
```

### Resultado Obtido

```
STATUS: [ ] ? PASSOU  [ ] ? FALHOU  [ ] ?? AVISO

Validação:      [ ] OK  [ ] Com avisos  [ ] Erro
Mensagens:      _____________________________________
Package Salvo:  [ ] SIM  [ ] NÃO
Exceções:       [ ] Nenhuma  [ ] Sim (qual?)

Observações:
_________________________________________________________________
_________________________________________________________________
```

---

## CENÁRIO 2: PAGINAÇÃO CURSOR

### Objetivo
Testar configuração de paginação usando cursor (token de continuação).

### Configuração

```
Base URL:              https://api.exemplo.com
Endpoint:              /v2/items
Pagination Type:       Cursor
Start Page:            1
Max Pages:             100

Expected Result:
?? Validação passa
?? Cursor pronto para uso
?? Sem limite de páginas
```

### Procedimento

```
1. [ ] Abrir wizard
2. [ ] Aba General:
   [ ] Base URL: https://api.exemplo.com
   [ ] Endpoint: /v2/items
   [ ] Page Size: 1000 (default cursor)
3. [ ] Aba Paginação:
   [ ] Type: Cursor
   [ ] Start Page: 1
   [ ] Max Pages: 100
4. [ ] Clicar OK
5. [ ] Package salva
```

### Resultado Esperado

```
[ ] Type "Cursor" reconhecido
[ ] Start Page: 1 aceito
[ ] Max Pages: 100 aceito
[ ] Nenhuma validação negativa
[ ] Wizard fecha normalmente
```

### Resultado Obtido

```
STATUS: [ ] ? PASSOU  [ ] ? FALHOU  [ ] ?? AVISO

Cursor Type:    [ ] Reconhecido  [ ] Erro
Start Page:     [ ] OK  [ ] Erro: ________________
Max Pages:      [ ] OK  [ ] Erro: ________________
Wizard Fecha:   [ ] SIM  [ ] NÃO

Observações:
_________________________________________________________________
_________________________________________________________________
```

---

## CENÁRIO 3: INCREMENTAL COM WATERMARK

### Objetivo
Testar carregamento incremental usando coluna watermark.

### Configuração

```
Base URL:              https://api.prod.com
Endpoint:              /v1/customers
Enable Incremental:    Checked (?)
Watermark Column:      updated_at
Source System:         ProductionAPI
Environment:           PRD

Expected Result:
?? Validação passa
?? Incremental ativado
?? Será usar updated_at
```

### Procedimento

```
1. [ ] Abrir wizard
2. [ ] Aba General:
   [ ] Base URL: https://api.prod.com
   [ ] Endpoint: /v1/customers
3. [ ] Aba Incremental:
   [ ] Enable Incremental: Checked
   [ ] Watermark Column: updated_at
   [ ] System: ProductionAPI
   [ ] Environment: PRD
4. [ ] Clicar OK
5. [ ] Verificar persistência
```

### Resultado Esperado

```
[ ] Incremental está "ativado"
[ ] Watermark Column: updated_at aceito
[ ] System: ProductionAPI salvo
[ ] Environment: PRD salvo
[ ] Validação passa
[ ] Sem erros bloqueadores
```

### Resultado Obtido

```
STATUS: [ ] ? PASSOU  [ ] ? FALHOU  [ ] ?? AVISO

Enable Incremental:     [ ] SIM  [ ] NÃO
Watermark Column:       ________________
System:                 ________________
Environment:            ________________
Validation:             [ ] OK  [ ] Erro

Observações:
_________________________________________________________________
_________________________________________________________________
```

---

## CENÁRIO 4: RAW STORAGE

### Objetivo
Testar armazenamento de JSON bruto com compressão e hash.

### Configuração

```
Raw Store Mode:        FileSystem
Raw Store Target:      C:\Data\Raw
Compress JSON:         Checked (?)
Hash JSON:             Checked (?)

Expected Result:
?? Validação passa
?? Modo FileSystem aceito
?? Compressão e Hash ativados
```

### Procedimento

```
1. [ ] Abrir wizard
2. [ ] Aba Storage:
   [ ] Mode: FileSystem
   [ ] Target: C:\Data\Raw
   [ ] Compress JSON: Checked
   [ ] Hash JSON: Checked
3. [ ] Clicar OK
4. [ ] Verificar se salva
```

### Resultado Esperado

```
[ ] Storage Mode: FileSystem aceito
[ ] Target path: C:\Data\Raw reconhecido
[ ] Compress JSON: Checked (visível)
[ ] Hash JSON: Checked (visível)
[ ] Validação passa
[ ] Sem erros
```

### Resultado Obtido

```
STATUS: [ ] ? PASSOU  [ ] ? FALHOU  [ ] ?? AVISO

Storage Mode:      [ ] OK  [ ] Erro: ____________
Target Path:       [ ] OK  [ ] Erro: ____________
Compress:          [ ] Checked  [ ] Unchecked
Hash:              [ ] Checked  [ ] Unchecked
Validation:        [ ] OK  [ ] Erro

Observações:
_________________________________________________________________
_________________________________________________________________
```

---

## CENÁRIO 5: RETRIES E RATE LIMIT

### Objetivo
Testar configuração de retry automático com exponential backoff e rate limit.

### Configuração

```
Max Retries:           5
Backoff Mode:          Exponential
Base Delay (ms):       1000
Rate Limit (rpm):      120
Timeout (seg):         180

Expected Result:
?? Todos valores aceitos
?? Backoff configurado
?? Rate limit em limites sensatos
```

### Procedimento

```
1. [ ] Abrir wizard
2. [ ] Aba Advanced:
   [ ] Max Retries: 5
   [ ] Backoff Mode: Exponential
   [ ] Base Delay: 1000 ms
   [ ] Rate Limit: 120 rpm
   [ ] Timeout: 180 seg
3. [ ] Validar todos campos
4. [ ] Clicar OK
```

### Resultado Esperado

```
[ ] Max Retries: 5 aceito
[ ] Backoff: Exponential reconhecido
[ ] Base Delay: 1000 ms aceito
[ ] Rate Limit: 120 rpm aceito
[ ] Timeout: 180 seg aceito
[ ] Validação passa
[ ] Sem avisos bloqueadores
```

### Resultado Obtido

```
STATUS: [ ] ? PASSOU  [ ] ? FALHOU  [ ] ?? AVISO

Max Retries:       [ ] OK  [ ] Erro
Backoff Mode:      [ ] OK  [ ] Erro
Base Delay:        [ ] OK  [ ] Erro
Rate Limit:        [ ] OK  [ ] Erro
Timeout:           [ ] OK  [ ] Erro
Overall:           [ ] OK  [ ] Aviso  [ ] Erro

Observações:
_________________________________________________________________
_________________________________________________________________
```

---

## ?? RESUMO GERAL

```
???????????????????????????????????????????????????????????
? Cenário                             ? Status   ? Tempo  ?
???????????????????????????????????????????????????????????
? 1. Paginação Offset                 ? [ ] ____ ? ___ min?
? 2. Paginação Cursor                 ? [ ] ____ ? ___ min?
? 3. Incremental com Watermark        ? [ ] ____ ? ___ min?
? 4. Raw Storage                      ? [ ] ____ ? ___ min?
? 5. Retries e Rate Limit             ? [ ] ____ ? ___ min?
???????????????????????????????????????????????????????????
? TOTAL                               ? [ ] ____ ? ___ min?
???????????????????????????????????????????????????????????

Taxa de Sucesso: ___%
Status Geral: [ ] ? OK  [ ] ?? AVISOS  [ ] ? ERROS
```

---

## ? DECISÃO FINAL

```
Todos os cenários funcionaram?
[ ] SIM (100%) ? Ir para Sign-off
[ ] COM RESSALVAS ? Documentar em ISSUES_LOG
[ ] NÃO ? Ver Issues e rejeitar

Pronto para deploy?
[ ] SIM
[ ] NÃO (motivo): ______________________________
```

---

## ?? NOTAS

```
_________________________________________________________________
_________________________________________________________________
_________________________________________________________________
```

---

## ?? ASSINATURA

```
Testador: ___________________________    Data: ______________
Revisor: ___________________________    Data: ______________
```

---

**Versão**: 3.0.5-BLOCO-5.5  
**Status**: ?? PRONTO PARA USAR


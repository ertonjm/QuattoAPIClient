# ?? BLOCO 5.5 - PRACTICAL EXAMPLES TESTING GUIDE

**Projeto**: Quatto API Client - Configuration Wizard  
**Fase**: 3 (Testes)  
**Bloco**: 5.5 - Exemplos Práticos  
**Status**: ? COMPLETO E PRONTO PARA USAR  

---

## ?? OVERVIEW

```
Objetivo: Validar 5 cenários práticos e realistas

5 Cenários:
?? CENÁRIO 1: Paginação Offset (15 min)
?? CENÁRIO 2: Paginação Cursor (15 min)
?? CENÁRIO 3: Incremental com Watermark (20 min)
?? CENÁRIO 4: Raw Storage (15 min)
?? CENÁRIO 5: Retries e Rate Limit (15 min)

Total: ~75 minutos (1.25 horas)
```

---

## ?? COMPONENTES ENTREGUES

```
1. TEST_EXAMPLES_TEMPLATE.md
   ?? 5 cenários com procedimentos

2. ISSUES_LOG_TEMPLATE.md
   ?? Log profissional de problemas

3. EVIDENCE_TEMPLATE.md
   ?? Evidências dos testes

4. SIGN_OFF_TEMPLATE.md
   ?? Aprovação final

5. BLOCO_5.5_TESTING_GUIDE.md (este)
   ?? Guia de uso completo

TOTAL: 5 documentos (~50 KB)
```

---

## ?? CENÁRIO 1: PAGINAÇÃO OFFSET (15 min)

### O que testar?

```
Verificar se o wizard aceita configuração de paginação
usando offset (número de página).
```

### Configuração

```
Base URL:           https://api.gladium.com
Endpoint:           /v1/orders
Pagination Type:    Offset
Page Size:          500
Start Page:         1
Max Pages:          0 (sem limite)
```

### Passos

```
1. Abrir wizard
2. Preencher aba General
3. Ir para aba Paginação
4. Type = Offset
5. Start Page = 1
6. Max Pages = 0
7. OK ? Salvar
```

### O que esperar

```
? Validação aceita Offset
? Sem mensagens de erro
? Nenhum aviso bloqueador
? Package salva normalmente
```

### Se algo errado

```
? Offset não reconhecido?
   ?? Verificar tipo de dados
   ?? Registrar em ISSUES_LOG

? Validação rejeita?
   ?? Qual a mensagem de erro?
   ?? Documentar exatamente
```

---

## ?? CENÁRIO 2: PAGINAÇÃO CURSOR (15 min)

### O que testar?

```
Verificar se o wizard aceita paginação tipo Cursor
(token de continuação para grandes datasets).
```

### Configuração

```
Base URL:           https://api.exemplo.com
Endpoint:           /v2/items
Pagination Type:    Cursor
Page Size:          1000
Start Page:         1
Max Pages:          100
```

### Passos

```
1. Abrir wizard
2. Aba General ? preencher URL + Endpoint
3. Aba Paginação
4. Type = Cursor
5. Start Page = 1
6. Max Pages = 100
7. OK ? Salvar
```

### O que esperar

```
? Cursor reconhecido como tipo válido
? Sem erros de validação
? Sem avisos bloqueadores
? 100 como Max Pages aceito
? Package salva
```

### Se algo errado

```
? Cursor não reconhecido?
   ?? É tipo suportado?
   ?? Verificar enum PAGINATION_TYPE

? Erro ao salvar?
   ?? Qual a mensagem?
   ?? Registrar em ISSUES_LOG
```

---

## ?? CENÁRIO 3: INCREMENTAL COM WATERMARK (20 min)

### O que testar?

```
Verificar carregamento incremental usando coluna
watermark para rastrear mudanças.
```

### Configuração

```
Base URL:              https://api.prod.com
Endpoint:              /v1/customers
Enable Incremental:    Checked (?)
Watermark Column:      updated_at
Source System:         ProductionAPI
Environment:           PRD
```

### Passos

```
1. Abrir wizard
2. Aba General ? preencher URL + Endpoint
3. Aba Incremental:
   - Ativar checkbox "Enable Incremental"
   - Watermark Column: updated_at
   - System: ProductionAPI
   - Environment: PRD
4. OK ? Salvar
5. Fechar Package
6. Reabrir Package
7. Verificar se tudo persistiu
```

### O que esperar

```
? Incremental checkbox aparece
? Watermark Column: updated_at aceito
? System: ProductionAPI salvo
? Environment: PRD salvo
? Ao reabrir, tudo persiste
? Sem erros de validação
```

### Se algo errado

```
? Checkbox desaparece ao reabrir?
   ?? Persistência quebrada
   ?? BLOQUEADOR potencial

? Watermark Column não aceita "updated_at"?
   ?? Validação muito restritiva?
   ?? Registrar problema

? Valores não persistem?
   ?? Incremental data não salva
   ?? AVISO importante
```

---

## ?? CENÁRIO 4: RAW STORAGE (15 min)

### O que testar?

```
Verificar configuração de armazenamento de JSON bruto
com compressão e hash de detecção de mudanças.
```

### Configuração

```
Raw Store Mode:        FileSystem
Raw Store Target:      C:\Data\Raw
Compress JSON:         Checked (?)
Hash JSON:             Checked (?)
```

### Passos

```
1. Abrir wizard
2. Aba Storage:
   - Mode: FileSystem
   - Target: C:\Data\Raw
   - Compress JSON: Check
   - Hash JSON: Check
3. OK ? Salvar
4. Verificar visualmente se checkboxes estão checked
```

### O que esperar

```
? FileSystem mode reconhecido
? Target path: C:\Data\Raw aceito
? Checkbox Compress: Checked
? Checkbox Hash: Checked
? Validação passa
? Sem erros
```

### Se algo errado

```
? Target path inválido?
   ?? Path checking muito restritivo?
   ?? Registrar erro

? Checkboxes desaparecem?
   ?? UI bug
   ?? Registrar com screenshot

? Compressão não funciona?
   ?? Testar se gera arquivo menor
   ?? Se sim, problema apenas de config
```

---

## ?? CENÁRIO 5: RETRIES & RATE LIMIT (15 min)

### O que testar?

```
Verificar configuração de retry automático com
exponential backoff e rate limiting.
```

### Configuração

```
Max Retries:           5
Backoff Mode:          Exponential
Base Delay (ms):       1000
Rate Limit (rpm):      120
Timeout (seg):         180
```

### Passos

```
1. Abrir wizard
2. Aba Advanced:
   - Max Retries: 5
   - Backoff Mode: Exponential
   - Base Delay: 1000 ms
   - Rate Limit: 120 rpm
   - Timeout: 180 seg
3. Validar cada campo
4. OK ? Salvar
5. Fechar e reabrir
6. Verificar persistência
```

### O que esperar

```
? Max Retries: 5 aceito
? Backoff: Exponential reconhecido
? Base Delay: 1000 ms aceito
? Rate Limit: 120 rpm (sensato, não alerta)
? Timeout: 180 seg aceito
? Validação passa sem avisos críticos
? Todos valores persistem ao reabrir
```

### Se algo errado

```
? Algum valor não persiste?
   ?? Persistência incompleta
   ?? AVISO importante

? Rate Limit gera aviso?
   ?? 120 rpm é dentro do range?
   ?? Verificar se validação está certa

? Backoff não reconhecido?
   ?? Tipo "Exponential" é válido?
   ?? Registrar tipo recusado
```

---

## ?? WORKFLOW COMPLETO

```
INÍCIO (Setup)
  ?
  ??? CENÁRIO 1: Offset (15 min)
  ?    ?? Preencher TEST_EXAMPLES
  ?
  ??? CENÁRIO 2: Cursor (15 min)
  ?    ?? Preencher TEST_EXAMPLES
  ?
  ??? CENÁRIO 3: Incremental (20 min)
  ?    ?? Preencher TEST_EXAMPLES
  ?
  ??? CENÁRIO 4: Storage (15 min)
  ?    ?? Preencher TEST_EXAMPLES
  ?
  ??? CENÁRIO 5: Retries (15 min)
  ?    ?? Preencher TEST_EXAMPLES
  ?
  ??? ANÁLISE (15 min)
  ?    ?? Revisar 5 cenários
  ?    ?? Calcular taxa de sucesso
  ?    ?? Identificar issues
  ?
  ??? SIGN-OFF (15 min)
  ?    ?? Preencher SIGN_OFF
  ?    ?? Coletar 3 assinaturas
  ?    ?? Marcar FASE 3 como 100%
  ?
  ??? FIM

TOTAL: ~1.5-2 horas
```

---

## ? CHECKLIST ANTES DE COMEÇAR

```
Ambiente:
[ ] Visual Studio com SSDT aberto
[ ] SQL Server instalado
[ ] Componente registrado
[ ] Release build recente
[ ] Acesso a APIs de teste (se testando realmente)

Templates:
[ ] 4 templates salvos e acessíveis
[ ] Guia (este arquivo) lido

Pessoal:
[ ] Testador disponível 1.5-2h
[ ] Revisor pronto quando terminar
[ ] Product Manager para sign-off

Pronto? Começa os cenários!
```

---

## ?? PONTOS CRÍTICOS

```
?? IMPORTANTE:

1. Cada cenário é independente
   ?? Você pode testá-los em qualquer ordem

2. Persistência é crítica
   ?? Fechar e reabrir package para verificar

3. Cenário 3 (Incremental) é o mais complexo
   ?? Mais tempo = mais pontos de falha

4. Cenário 5 (Retries) testa configuração avançada
   ?? Valores têm ranges específicos

5. Coletar evidências enquanto testa
   ?? Não deixar para depois
```

---

## ?? CONFORMIDADE

```
Target: 100% (5/5 cenários passando)

Significa:
?? Todos 5 cenários funcionando ?
?? 0 bloqueadores
?? Validação completa
?? Pronto para deploy

Se < 100% (ex: 4/5):
?? Documentar qual falhou
?? Se é bloqueador ou não
?? Decidir se bloqueia deploy
```

---

## ?? PRÓXIMOS PASSOS

```
Após conclusão de BLOCO 5.5:

? Se TODOS 5 CENÁRIOS PASSAREM:
   ?? FASE 3: 100% COMPLETA ?
   ?? PROJETO: 100% COMPLETO ?
   ?? Conformidade: 95%+ ?
   ?? PRONTO PARA DEPLOY FINAL ?

? Se ALGUNS FALHAM:
   ?? Registrar em ISSUES_LOG
   ?? Decidir se bloqueia ou não
   ?? Se não bloqueia: prosseguir
   ?? Se bloqueia: aguardar fixes

? Se BLOQUEADORES CRÍTICOS:
   ?? Não marcar FASE 3 como 100%
   ?? Aguardar resolução
```

---

## ?? IMPACTO FINAL DO PROJETO

```
Após BLOCO 5.5 (todos cenários OK):

FASE 1: 100% ? (9.4h)
FASE 2: 100% ? (4.1h)
FASE 3: 100% ? (5.5-6.5h)

PROJETO: 100% ? (~18-20 horas de 31-42h)
ECONOMIA: 11-24 horas ??

Conformidade: 100% ?
Qualidade: Enterprise-level ?
Deploy: PRONTO ?

Status: ?? PROJETO CONCLUÍDO COM SUCESSO!
```

---

## ? CONCLUSÃO

```
BLOCO 5.5 - Exemplos Práticos:

? 4 templates criados (50 KB)
? Cobertura completa (5 cenários)
? Procedimentos claros
? Fácil de executar
? ~1.5-2 horas total

Status: ?? PRONTO PARA USAR!

Após completar BLOCO 5.5:
?? FASE 3: 100%
?? PROJETO: 100%
?? CONCLUSÃO TOTAL ?
```

---

**Versão**: 3.0.5-BLOCO-5.5-GUIDE  
**Status**: ?? PRONTO PARA USAR  
**Próximo**: Executar testes ou ir para Consolidação Final  


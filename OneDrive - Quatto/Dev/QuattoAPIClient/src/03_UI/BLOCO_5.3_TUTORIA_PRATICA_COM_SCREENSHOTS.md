# ?? BLOCO 5.3 - TUTORIA PRÁTICA COM SCREENSHOTS

**Projeto**: Quatto API Client - Configuration Wizard  
**Fase**: 3 (Testes)  
**Bloco**: 5.3 - SSDT Wizard Testing  
**Tipo**: Tutoria Prática com Screenshots Esperadas  
**Status**: ? COMPLETO E PRONTO PARA USAR  

---

## ?? OBJETIVO

Executar 5 testes de UI/Functional no SSDT com passo-a-passo visual.

---

## ?? PRÉ-REQUISITOS CONFIRMADOS

```
? Visual Studio com SSDT
? SQL Server Integration Services
? .NET Framework 4.7.2
? Release build compilada
? Componente registrado
? Integration Services Package pronto
```

---

## ?? COMEÇAR

Tempo total: 2-3 horas (5 testes)

---

# TESTE 1: UI VISUAL (15 min)

## Objetivo
Verificar que o wizard abre e a UI está visualmente correta.

## Passo 1.1: Abrir o Package

```
AÇÃO:
1. Visual Studio ? Abrir Integration Services Package
2. Data Flow Task ? Adicionar componente (nosso wizard)
3. Direita do mouse ? Edit (abre wizard)

ESPERADO:
```
???????????????????????????????????????????????
? Quatto API Client Configuration Wizard      ? ? Título
???????????????????????????????????????????????
?  [General] [Pagination] [Incremental]...    ? ? 5 tabs
?                                             ?
?  General                                    ?
?  ???????????????????????????????????????   ?
?  ? Base URL:  [ __________________ ]   ?   ?
?  ? Endpoint:  [ __________________ ]   ?   ?
?  ? Page Size: [ __________________ ]   ?   ?
?  ???????????????????????????????????????   ?
?                                             ?
?  [ OK ]  [ Cancel ]  [ Apply ]              ? ? Buttons
???????????????????????????????????????????????

CHECKLIST:
[ ] Wizard abre sem erro
[ ] Forma 900x700 pixels (aprox)
[ ] 5 tabs visíveis
[ ] Tab "General" é o padrão
[ ] 3 campos (URL, Endpoint, PageSize) visíveis
[ ] 3 buttons na base (OK, Cancel, Apply)
```

## Passo 1.2: Verificar Cores e Layout

```
AÇÃO:
1. Observar cores dos GroupBoxes
2. Verificar fonte (Segoe UI)
3. Avaliar espaçamento

ESPERADO:
```
Cores Visíveis:
?? Azul (API Connection) ? Atual neste tab
?? Verde (Incremental) ? Não visível agora
?? Laranja (Storage)
?? Vermelho (Advanced)

Fontes:
?? Título: Segoe UI, 12pt, Bold
?? Labels: Segoe UI, 10pt, Normal
?? Buttons: Segoe UI, 10pt, Normal

Espaçamento:
?? Margens: ~10px de cada lado
?? Entre campos: ~10px
?? Overall: Profissional e organizado
```

CHECKLIST:
[ ] GroupBox azul aparece
[ ] Labels legíveis
[ ] Campos bem espaçados
[ ] Layout profissional

## Resultado Passo 1

```
STATUS: [ ] ? PASSOU  [ ] ? FALHOU  [ ] ?? AVISO

Observações:
_________________________________________________________________
_________________________________________________________________

Se FALHOU, ver TROUBLESHOOTING 1.1 abaixo
```

---

## ?? TROUBLESHOOTING TESTE 1

### Problema 1.1: Wizard não abre / Exception

```
ERRO:
"Exception thrown when opening wizard"

CAUSAS POSSÍVEIS:
1. Componente não registrado
   ? Solução: Re-registrar no Visual Studio
   ? Cmd: regasm.exe QuattoApiClient.dll

2. DLL corrompida ou desatualizada
   ? Solução: Recompilar release build
   ? Comando: msbuild /p:Configuration=Release

3. .NET Framework 4.7.2 não instalado
   ? Solução: Verificar via Windows ? Control Panel
   ? Confirmar: .NET Framework 4.7.2 ou superior

CHECKLIST:
[ ] Componente está em C:\Program Files\...\
[ ] DLL é recente (último 10 minutos)
[ ] .NET 4.7.2 está instalado
[ ] Visual Studio sem cache (Clear Temp)
```

### Problema 1.2: UI muito pequena ou distorcida

```
ERRO:
"Wizard abre mas UI está distorcida"

CAUSAS:
1. Resolução de tela muito alta (4K)
   ? Solução: Testar em resolução menor (1920x1080)

2. DPI scaling ativado
   ? Solução: Desativar DPI scaling na propriedade da app

3. Font não encontrada
   ? Solução: Instalar Segoe UI (vem com Windows)

CHECKLIST:
[ ] Resolução é 1920x1080 ou maior
[ ] DPI Scaling desativado
[ ] Segoe UI disponível
```

---

# TESTE 2: CONTROLES (20 min)

## Objetivo
Verificar que todos 54 controles funcionam e respondem corretamente.

## Passo 2.1: Testar Campos Editáveis

```
AÇÃO:
1. Clicar em campo "Base URL"
2. Digitar: https://api.test.com
3. Observar que aceita e armazena valor
4. Passar para próximo campo (Tab)
5. Repetir para "Endpoint": /v1/data
6. Repetir para "Page Size": 500

ESPERADO:
```
Antes:
?? Base URL: [ (vazio) ]
?? Endpoint: [ (vazio) ]
?? Page Size: [ (vazio) ]

Depois de preencher:
?? Base URL: [ https://api.test.com ]
?? Endpoint: [ /v1/data ]
?? Page Size: [ 500 ]

Comportamento:
?? Campos aceitam entrada
?? Valores permanecem após Tab
?? Sem mensagens de erro (ainda)
?? Cursor avança para próximo campo
```

CHECKLIST:
[ ] Pode digitar em todos 3 campos
[ ] Valores persistem na UI
[ ] Tab move para próximo campo
[ ] Sem erros ao digitar

## Passo 2.2: Testar Tooltips

```
AÇÃO:
1. Posicionar mouse em "Base URL" label
2. Aguardar ~500ms
3. Observar se tooltip aparece
4. Ler mensagem
5. Repetir para outros campos

ESPERADO:
```
Tooltip do Base URL:
???????????????????????????????????????
? URL base da API (ex: api.gladi.com) ?
???????????????????????????????????????

Tooltip do Endpoint:
???????????????????????????????????????
? Caminho do endpoint (ex: /v1/data)  ?
???????????????????????????????????????

Tooltip do Page Size:
???????????????????????????????????????
? Registros por página (padrão: 500)  ?
???????????????????????????????????????

Comportamento:
?? Tooltip aparece após 500ms
?? Texto legível
?? Posicionado corretamente
?? Desaparece ao sair
```

CHECKLIST:
[ ] Tooltip Base URL aparece
[ ] Tooltip Endpoint aparece
[ ] Tooltip Page Size aparece
[ ] Texto é útil e informativo
[ ] Não interfere na UI

## Passo 2.3: Testar Tabs

```
AÇÃO:
1. Clicar em tab "Pagination"
2. Observar mudança para novo tab
3. Voltar para tab "General"
4. Clicar em tab "Incremental"
5. Observar mudança

ESPERADO:
```
Antes (General):
?? Base URL, Endpoint, Page Size visíveis
?? Pagination, Incremental, Storage, Advanced ocultos

Depois de clicar "Pagination":
?? Pagination tab em foreground
?? Controles de paginação visíveis
?? General tab em background

Depois de clicar "General":
?? Volta aos campos originais
?? Tab "General" destacado
?? State do tab anterior preserved
```

CHECKLIST:
[ ] Tabs são clicáveis
[ ] Conteúdo muda ao clicar
[ ] Todos 5 tabs funcionam
[ ] Volta ao anterior sem perder dados
[ ] Sem erros de navegação

## Resultado Passo 2

```
STATUS: [ ] ? PASSOU  [ ] ? FALHOU  [ ] ?? AVISO

Controles testados: 54
Funcionando: ___/54

Observações:
_________________________________________________________________
_________________________________________________________________
```

---

# TESTE 3: VALIDAÇÃO (20 min)

## Objetivo
Verificar que validação funciona (erros e avisos).

## Passo 3.1: Testar Validação Bloqueante

```
AÇÃO:
1. Limpar campo "Base URL" (deixar vazio)
2. Clicar em "Endpoint" (sair do campo)
3. Observar comportamento

ESPERADO:
```
Visual:
?? Campo Base URL fica com fundo MistyRose (rosa claro)
?? Mensagem de erro aparece
?? Botão OK desativado

Mensagem:
???????????????????????????????????????????????????
? ??  Base URL é obrigatório (ex: api.test.com)   ?
???????????????????????????????????????????????????

Comportamento:
?? Não pode clicar OK
?? Can clicar Cancel (sair)
?? Erro desaparece ao corrigir
?? Campo volta ao normal quando OK
```

CHECKLIST:
[ ] Campo fica MistyRose
[ ] Mensagem de erro clara
[ ] OK button fica desativado
[ ] Erro desaparece ao corrigir

## Passo 3.2: Testar Validação com Aviso

```
AÇÃO:
1. Na aba "Advanced"
2. Configurar "Max Retries": 10 (muito alto)
3. Sair do campo
4. Observar

ESPERADO:
```
Visual:
?? Campo fica com fundo amarelo (aviso, não erro)
?? Mensagem de aviso (não bloqueante)
?? Botão OK permanece ativado

Mensagem:
???????????????????????????????????????????????????
? ??  Max Retries 10 é alto. Considere 3-5.      ?
???????????????????????????????????????????????????

Comportamento:
?? Aviso, não erro
?? Pode clicar OK mesmo assim
?? Apenas informativo
?? Pode ignorar com segurança
```

CHECKLIST:
[ ] Campo com fundo diferente para aviso
[ ] Mensagem informativa
[ ] OK button permanece ativado
[ ] Pode prosseguir com aviso

## Resultado Passo 3

```
STATUS: [ ] ? PASSOU  [ ] ? FALHOU  [ ] ?? AVISO

Validações testadas: 11 (7 bloqueantes + 4 avisos)
Funcionando: ___/11

Observações:
_________________________________________________________________
_________________________________________________________________
```

---

# TESTE 4: PERSISTÊNCIA (20 min)

## Objetivo
Verificar que valores salvam e carregam corretamente.

## Passo 4.1: Configurar e Salvar

```
AÇÃO:
1. Preencher campos conforme:
   ?? Base URL: https://api.prod.com
   ?? Endpoint: /v2/customers
   ?? Page Size: 1000
   
2. Ir para aba "Pagination":
   ?? Pagination Type: Offset
   ?? Start Page: 1
   ?? Max Pages: 500
   
3. Ir para aba "Incremental":
   ?? Enable: Checked
   ?? Watermark: created_at
   
4. Ir para aba "Storage":
   ?? Mode: FileSystem
   ?? Target: C:\Data\Raw
   
5. Ir para aba "Advanced":
   ?? Max Retries: 5
   ?? Timeout: 180
   
6. Clicar OK

ESPERADO:
```
Ação ? Resultado:
?? Todos campos aceitos ?
?? Validação passou ?
?? Wizard fechou ?
?? Package salvou ?
?? Sem mensagens de erro ?
```

CHECKLIST:
[ ] Todos campos preenchidos
[ ] Validação passou
[ ] Wizard fechou sem erro
[ ] Package salvo (no status bar)

## Passo 4.2: Reabrir e Verificar

```
AÇÃO:
1. Fechar Package (File ? Close ? Save)
2. Fechar SSDT
3. Reabrir Visual Studio
4. Abrir Package novamente
5. Double-click no nosso componente
6. Wizard abre novamente

VERIFICAR CADA CAMPO:

ESPERADO:
```
Aba General:
?? Base URL: https://api.prod.com ?
?? Endpoint: /v2/customers ?
?? Page Size: 1000 ?

Aba Pagination:
?? Type: Offset ?
?? Start Page: 1 ?
?? Max Pages: 500 ?

Aba Incremental:
?? Enable: Checked ?
?? Watermark: created_at ?

Aba Storage:
?? Mode: FileSystem ?
?? Target: C:\Data\Raw ?

Aba Advanced:
?? Max Retries: 5 ?
?? Timeout: 180 ?

RESULTADO: 100% dos valores persistiram!
```

CHECKLIST:
[ ] General tab: todos valores OK
[ ] Pagination tab: todos valores OK
[ ] Incremental tab: todos valores OK
[ ] Storage tab: todos valores OK
[ ] Advanced tab: todos valores OK
[ ] Nenhum valor perdido

## Resultado Passo 4

```
STATUS: [ ] ? PASSOU  [ ] ? FALHOU  [ ] ?? AVISO

Propriedades testadas: 28
Persistidas corretamente: ___/28

Observações:
_________________________________________________________________
_________________________________________________________________
```

---

# TESTE 5: REAL-TIME VALIDATION (15 min)

## Objetivo
Verificar que validação acontece ao sair do campo (não ao digitar).

## Passo 5.1: Digitar Valor Inválido

```
AÇÃO:
1. Vá para aba "Advanced"
2. Campo "Max Retries"
3. Digitar: abc (valor inválido)
4. OBSERVAR SE ERRO APARECE (não deve)
5. Sair do campo (Tab ou Enter)
6. AGORA o erro deve aparecer

ESPERADO:
```
Enquanto digita:
?? Sem erro ainda
?? Campo aceita entrada
?? Esperando validação (LostFocus)

Após sair (Tab):
?? Campo fica MistyRose
?? Erro aparece:
?  "Max Retries deve ser número"
?? OK button desativado

Comportamento:
?? Validação é real-time (ao sair)
?? Não valida enquanto digita
?? Feedback instantâneo após sair
?? User pode corrigir imediatamente
```

CHECKLIST:
[ ] Sem erro enquanto digita
[ ] Erro aparece ao sair do campo
[ ] Feedback é instantâneo
[ ] Mensagem é clara

## Resultado Passo 5

```
STATUS: [ ] ? PASSOU  [ ] ? FALHOU  [ ] ?? AVISO

Real-time validations: 6 campos
Funcionando: ___/6

Observações:
_________________________________________________________________
_________________________________________________________________
```

---

# ?? RESUMO DOS 5 TESTES

```
??????????????????????????????????????????????????
? Teste                               ? Status   ?
??????????????????????????????????????????????????
? 1. UI Visual                        ? [ ]      ?
? 2. Controles (54)                   ? [ ]      ?
? 3. Validação (11)                   ? [ ]      ?
? 4. Persistência (28 props)          ? [ ]      ?
? 5. Real-time Validation (6)         ? [ ]      ?
??????????????????????????????????????????????????
? TOTAL                               ? [ ]      ?
??????????????????????????????????????????????????

Taxa de Sucesso:
?? 5/5 = 100% ?
?? 4/5 = 80% ??
?? 3/5 = 60% ?
?? <3/5 = Bloqueador ?
```

---

# ?? TROUBLESHOOTING RÁPIDO

## Problema: Wizard congela ao abrir

```
SOLUÇÃO:
1. Fechar Visual Studio
2. Limpar temp: %TEMP% ? Delete
3. Reabrir Visual Studio
4. Tentar novamente
```

## Problema: Valores não persistem

```
SOLUÇÃO:
1. Verificar que clicou OK (não Cancel)
2. Verificar que Package foi salvo
3. Verificar que reabriu o Package
4. Registrar em ISSUES_LOG se persistir
```

## Problema: Validação não funciona

```
SOLUÇÃO:
1. Verificar que saiu do campo (Tab/Enter)
2. Não validação ao digitar (é normal)
3. Conferir mensagem de erro
4. Se nada, registrar em ISSUES_LOG
```

---

# ?? PRÓXIMOS PASSOS

```
Após completar BLOCO 5.3:

[ ] Preencher TEST_RESULTS_TEMPLATE.md
[ ] Registrar issues em ISSUES_LOG (se houver)
[ ] Coletar evidências em EVIDENCE_SCREENSHOTS
[ ] Clicar OK para prosseguir
[ ] Ir para BLOCO 5.4 (Connection Manager)

ETA: 2-3 horas total para BLOCO 5.3
```

---

**Versão**: 3.0.5-BLOCO-5.3-TUTORIA  
**Status**: ?? PRONTO PARA USAR  
**Próximo**: Executar BLOCO 5.3 agora!  


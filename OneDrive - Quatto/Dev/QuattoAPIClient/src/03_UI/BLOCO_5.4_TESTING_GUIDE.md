# ?? BLOCO 5.4 - CONNECTION MANAGER TESTING GUIDE

**Projeto**: Quatto API Client - Configuration Wizard  
**Fase**: 3 (Testes)  
**Bloco**: 5.4 - Connection Manager Integration  
**Status**: ? COMPLETO E PRONTO PARA USAR  

---

## ?? OVERVIEW

```
Objetivo: Validar integração do wizard com SSIS Connection Manager

3 Testes Principais:
?? TESTE 1: CM Carrega no ComboBox (15 min)
?? TESTE 2: CM Integra com Wizard (30 min)
?? TESTE 3: Autenticação & Sincronização (30 min)

Total: ~75 minutos (1.25 horas)
```

---

## ?? COMPONENTES ENTREGUES

```
1. TEST_PROCEDURES_TEMPLATE.md
   ?? 3 testes com procedimentos completos

2. ISSUES_LOG_TEMPLATE.md
   ?? Log profissional de problemas

3. EVIDENCE_TEMPLATE.md
   ?? Evidências visuais dos testes

4. SIGN_OFF_TEMPLATE.md
   ?? Aprovação e decisão final

5. BLOCO_5.4_TESTING_GUIDE.md (este)
   ?? Guia de uso completo

TOTAL: 5 documentos (~40 KB)
```

---

## ?? TESTE 1: CM CARREGA COMBOBOX (15 min)

### O que testar?

```
Verifica que Connection Managers aparecem na ComboBox do wizard
quando você abre o componente no SSDT.
```

### Pré-requisitos

```
? Visual Studio com SSDT instalado
? Integration Services Package aberto
? 1-2 Connection Managers criados:
   ?? Ex: oledb_test1, oledb_test2
? Nosso componente adicionado ao package
```

### Passos

```
1. SSDT ? Data Flow Task ? Add Source (nosso componente)
2. Direita ? Edit (abre wizard)
3. Procurar ComboBox "Conexão" em General tab
4. Verificar se lista está preenchida
5. Esperar ~2-3 segundos
6. Observar se CMs aparecem
```

### O que esperar

```
? ComboBox não vazio
? Lista mostra CMs criados
? Pode clicar e selecionar
? Sem mensagens de erro
? Sem delay excessivo
```

### Se algo errado

```
? ComboBox vazio?
   ?? Verificar se CMs realmente existem
   ?? Verificar se SSIS types estão comentados
   ?? Checar console para exceções

? Exception ao abrir?
   ?? Ver details na console do Visual Studio
   ?? Registrar em ISSUES_LOG

? Muito lento?
   ?? >2 segundos = problema
   ?? Documentar tempo exato
```

---

## ?? TESTE 2: CM INTEGRA COM WIZARD (30 min)

### O que testar?

```
Verifica que selecionando um CM, o wizard se comporta corretamente
e sincroniza valores conforme esperado.
```

### Pré-requisitos

```
? TESTE 1 passou
? 2+ CMs disponíveis
? Wizard aberto
```

### Passos

```
1. Abrir ComboBox "Conexão"
2. Selecionar primeiro CM (ex: oledb_test1)
   ?? Aguardar ~1 segundo
   ?? Observar UI
3. Preencher outros campos:
   ?? Base URL: https://api.test.com
   ?? Endpoint: /v1/data
   ?? Page Size: 500
4. Selecionar outro CM (ex: oledb_test2)
   ?? Base URL, Endpoint, Page Size não devem mudar
5. Clicar OK ? Package salva
6. Sem erros?
```

### O que esperar

```
? Ao selecionar CM:
   ?? Sem freeze da UI
   ?? ComboBox mostra seleção
   ?? Responde em <2 segundos

? Outros campos não afetados:
   ?? Base URL permanece igual
   ?? Endpoint permanece igual
   ?? Configurações não mudam

? Clicar OK funciona:
   ?? Validação passa
   ?? Wizard fecha
   ?? Package salva sem erro
```

### Se algo errado

```
? UI congela ao selecionar CM?
   ?? Performance issue
   ?? Documentar tempo exato
   ?? Registrar em ISSUES_LOG

? Outro campo muda quando seleciona CM?
   ?? Integração quebrada
   ?? Investigar no código
   ?? BLOQUEADOR potencial

? Erro ao clicar OK?
   ?? Ver mensagem de erro
   ?? Registrar problema completo
```

---

## ?? TESTE 3: AUTENTICAÇÃO & SINCRONIZAÇÃO (30 min)

### O que testar?

```
Verifica que valores salvam corretamente e carregam quando
você reabre o package, incluindo credenciais do CM.
```

### Pré-requisitos

```
? TESTE 2 passou
? 1 CM com credenciais (opcional)
? Wizard aberto
```

### Passos - PARTE 1: CONFIGURAR E SALVAR

```
1. Selecionar um CM no ComboBox
   ?? Ex: oledb_test1

2. Preencher campos:
   ?? Base URL: https://api.prod.com
   ?? Endpoint: /v2/dados
   ?? Page Size: 500
   ?? Timeout: 120

3. Ir para outros tabs e preencher:
   ?? Incremental: [ ] ativar
   ?? Storage: deixar defaults
   ?? Advanced: deixar defaults

4. Clicar OK
   ?? Deve fechar wizard
   ?? Deve salvar package

5. Aguardar ~2 segundos
```

### Passos - PARTE 2: REABRIR E VERIFICAR

```
1. Fechar Package (File ? Close)
   ?? Escolher "Save" se pergunta

2. Reabrir Package (File ? Recent ? o arquivo)
   ?? Aguardar carregar

3. Double-click no nosso componente
   ?? Wizard abre novamente

4. VERIFICAR CADA CAMPO:

   ?? Conexão: ainda é oledb_test1?
   ?  [ ] SIM  [ ] NÃO (mudou para quê?)
   ?
   ?? Base URL: ainda https://api.prod.com?
   ?  [ ] SIM  [ ] NÃO (mudou para quê?)
   ?
   ?? Endpoint: ainda /v2/dados?
   ?  [ ] SIM  [ ] NÃO
   ?
   ?? Page Size: ainda 500?
   ?  [ ] SIM  [ ] NÃO
   ?
   ?? Timeout: ainda 120?
      [ ] SIM  [ ] NÃO

5. Se tudo [ ] SIM: teste passa ?
```

### O que esperar

```
? CM selecionado persiste
? Base URL persiste
? Endpoint persiste
? Page Size persiste
? Timeout persiste
? Nenhuma corrupção de dados
? 100% dos valores carregam
```

### Se algo errado

```
? Alguns campos não persistem?
   ?? Persistência incompleta
   ?? Listar quais campos faltam
   ?? AVISO (não bloqueador)

? Alguns campos com valores diferentes?
   ?? Corrupção de dados
   ?? BLOQUEADOR potencial

? Package não salva?
   ?? Erro crítico
   ?? BLOQUEADOR absoluto

? Conexão não conecta?
   ?? Verificar credenciais do CM
   ?? Verificar configuração do CM
   ?? Pode ser problema do CM, não do wizard
```

---

## ?? WORKFLOW COMPLETO

```
INÍCIO (Setup)
  ?
  ??? TESTE 1: CM Carrega ComboBox (15 min)
  ?    ?? Preencher TEST_PROCEDURES
  ?    ?? Coletar evidência em EVIDENCE
  ?
  ??? TESTE 2: CM Integra Wizard (30 min)
  ?    ?? Preencher TEST_PROCEDURES
  ?    ?? Coletar evidência em EVIDENCE
  ?    ?? Se houver issue: registrar em ISSUES_LOG
  ?
  ??? TESTE 3: Autenticação & Sincronização (30 min)
  ?    ?? Preencher TEST_PROCEDURES
  ?    ?? Coletar evidência em EVIDENCE
  ?    ?? Se houver issue: registrar em ISSUES_LOG
  ?
  ??? ANÁLISE (15 min)
  ?    ?? Revisar todos os 3 testes
  ?    ?? Calcular taxa de sucesso
  ?    ?? Identificar bloqueadores
  ?
  ??? SIGN-OFF (15 min)
  ?    ?? Preencher SIGN_OFF_TEMPLATE
  ?    ?? Coletar 3 assinaturas
  ?    ?? Decidir se prossegue ou não
  ?
  ??? FIM

TOTAL: ~1.5-2 horas
```

---

## ? CHECKLIST ANTES DE COMEÇAR

```
Ambiente:
[ ] Visual Studio com SSDT aberto
[ ] SQL Server instalado e rodando
[ ] Componente registrado e visível
[ ] Release build recente (~5 min)
[ ] 1-2 Connection Managers criados

Templates:
[ ] 4 templates salvos e acessíveis
[ ] Guia (este arquivo) lido

Pessoal:
[ ] Testador disponível 1.5-2h
[ ] Revisor pronto quando terminar
[ ] Product Manager para sign-off

Pronto? Começa os testes!
```

---

## ?? PONTOS CRÍTICOS

```
?? IMPORTANTE:

1. Não esquecer de SALVAR o package após OK
   ?? Caso contrário não há persistência para testar

2. Fechar E REABRIR o package
   ?? Não basta fechar o wizard

3. Coletar evidências enquanto testa
   ?? Não deixar para depois

4. Se houver erro, anotar EXATAMENTE o que diz
   ?? Será importante para debugging

5. Timing importa:
   ?? Notar se algo fica muito lento (>2s)
```

---

## ?? CONFORMIDADE

```
Target: 95%+ de conformidade

Significa:
?? Todos 3 testes passando ?
?? ?1 issue de baixa prioridade
?? 0 bloqueadores
?? Pronto para BLOCO 5.5

Se < 95%:
?? Documentar tudo em ISSUES_LOG
?? Decidir se bloqueia ou não
?? Registrar recomendação em SIGN_OFF
```

---

## ?? PRÓXIMOS PASSOS

```
Após conclusão de BLOCO 5.4:

? Se APROVADO:
   ?? Prosseguir para BLOCO 5.5 (Exemplos)
   ?? 5 cenários práticos
   ?? ~1-2 horas

? Se COM RESSALVAS:
   ?? Documentar workarounds
   ?? Prosseguir para BLOCO 5.5 mesmo assim
   ?? Schedule fixes para depois

? Se BLOQUEADORES:
   ?? Registrar bloqueadores
   ?? Aguardar fixes
   ?? Não prosseguir até resolver
```

---

## ?? IMPACTO NO PROJETO

```
Após BLOCO 5.4:

FASE 3: 60% ? 80% (5.3-5.4 concluído)
PROJETO: ~62% ? ~65% (20.2h de 31-42h)

Conformidade:
?? Atual: 95%
?? Após 5.4: 95%+
?? Target: 95%+ ?

ETA:
?? BLOCO 5.5: 1-2 horas
?? Final: 1-2 dias
?? Total projeto: ~18-20 horas
```

---

## ? CONCLUSÃO

```
BLOCO 5.4 - Connection Manager Integration:

? 4 templates criados (40 KB)
? Cobertura completa (3 testes)
? Procedimentos claros
? Fácil de executar
? ~1.5-2 horas total

Status: ?? PRONTO PARA USAR!
```

---

**Versão**: 3.0.4-BLOCO-5.4-GUIDE  
**Status**: ?? PRONTO PARA USAR  
**Próximo**: Executar testes ou ir para BLOCO 5.5  


<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!-- don't do anything to other versions -->
	<xsl:template match="configuration[@version='8']">
		<configuration version="9">
			<xsl:apply-templates mode ="Migrate8To9"/>
		</configuration>
	</xsl:template>

	<xsl:template match="@*|node()" mode="Migrate8To9">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"  mode="Migrate8To9"/>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="@*|node()" mode="identity">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"  mode="identity"/>
		</xsl:copy>
	</xsl:template>

	<!-- ============= actual changing starts here ================ -->
	<xsl:template match="task[@taskName='Dictionary']" mode="Migrate8To9">
		<task taskName="Dictionary">
			<xsl:attribute name="visible">
				<xsl:value-of select="current()/@visible"/>
			</xsl:attribute>
			<meaningField>definition</meaningField>
		</task>
	</xsl:template>

	<xsl:template match="fields/field" mode="Migrate8To9">
		<field>
		<xsl:copy-of select="./*"/>
		<xsl:choose>
			<xsl:when test="current()/fieldName='definition'">
				<meaningField>True</meaningField>
			</xsl:when>
			<xsl:otherwise>
				<meaningField>False</meaningField>
			</xsl:otherwise>
		</xsl:choose>
		</field>
	</xsl:template>
<!--
	<xsl:template match="fields/field" mode="Migrate8To9">
		<field>
			<xsl:apply-templates>
				<xsl:sort select="name()"/>
			</xsl:apply-templates>
		</field>
	</xsl:template>
-->
</xsl:stylesheet>